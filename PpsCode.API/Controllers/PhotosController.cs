using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PpsCode.API.Data;
using PpsCode.API.Dtos;
using PpsCode.API.Helpers;
using PpsCode.API.Models;

namespace PpsCode.API.Controllers
{
  [Authorize]
  [Route("api/users/{userId}/photos")]
  [ApiController]
  public class PhotosController : ControllerBase
  {
    private readonly IDevRepository _repo;
    private readonly IMapper _mapper;
    private readonly IOptions<CloudinarySettings> _cloudinaryConfig;
    private Cloudinary _cloudinary;

    public PhotosController(IDevRepository repo, IMapper mapper,
    IOptions<CloudinarySettings> cloudinaryConfig)
    {
      _cloudinaryConfig = cloudinaryConfig;
      _mapper = mapper;
      _repo = repo;

      Account acc = new Account(
          _cloudinaryConfig.Value.CloudName,
          _cloudinaryConfig.Value.ApiKey,
          _cloudinaryConfig.Value.ApiSecret
      );

      _cloudinary = new Cloudinary(acc);

    }

    [HttpGet("{id}", Name = "GetPhoto")]
    public async Task<IActionResult> GetPhoto(int id)
    {
        var photoFromRepo = await _repo.GetPhoto(id);

        var photo = _mapper.Map<PhotoForReturnDto>(photoFromRepo);

        return Ok(photo);
    }

    [HttpPost]
    public async Task<IActionResult> AddPhotoForUser(int userId, 
        [FromForm]PhotoForCreationDto photoForCreationDto)
    {
     // check user attempting to update there profile mathes the token the server receiving   
      if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
      {
          return Unauthorized();
      } 
      var userFromRepo = await _repo.GetUser(userId);
      var file = photoForCreationDto.File;
    //   storing result get back from cloudinary
      var uploadResult = new ImageUploadResult();
      
      if (file.Length > 0)
      {
          using (var stream = file.OpenReadStream()) // read file into memory
          {
              var uploadParams = new ImageUploadParams()
              {
                  File = new FileDescription(file.Name, stream), // specify file
                // transform image focus on face and crop the area
                  Transformation = new Transformation()
                    .Width(500).Height(500).Crop("fill").Gravity("face") 
              };
              uploadResult = _cloudinary.Upload(uploadParams);
          }
      }
      photoForCreationDto.Url = uploadResult.Uri.ToString();
      photoForCreationDto.PublicId = uploadResult.PublicId;

      //   map photoCreationDto into photo
      var photo = _mapper.Map<Photo>(photoForCreationDto);

      if (!userFromRepo.Photos.Any(u => u.IsMain)) // if return false user doesnt have main photo 
      {
      photo.IsMain = true;   
      }
      userFromRepo.Photos.Add(photo);


      if (await _repo.SaveAll()) // if the first photo user uploading ? set this photo to be main photo
      {
          var photoToReturn = _mapper.Map<PhotoForReturnDto>(photo); 
          return CreatedAtRoute("GetPhoto", new { id = photo.Id}, photoToReturn);
      }
      return BadRequest("Could not add the photo");
      }

    [HttpPost("{id}/setMain")]
    public async Task<IActionResult> SetMainPhoto(int userId, int id)
    {
        // check user attempting to update there profile mathes the token the server receiving   
      if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
      {
          return Unauthorized();
      }
      var user = await _repo.GetUser(userId);

      if (!user.Photos.Any(p => p.Id == id)) // if id not match any user photo collection
      {
          return Unauthorized();
      }
      var photoFromRepo = await _repo.GetPhoto(id);
      if (photoFromRepo.IsMain) // main photo
      {
          return BadRequest("this is already the main photo");
      }
      var currentMainPhoto = await _repo.GetMainPhotoForUser(userId);
      currentMainPhoto.IsMain = false;

      photoFromRepo.IsMain = true;

      if(await _repo.SaveAll()){
        return NoContent();
      }
    
     return BadRequest("Could not ser photo to main");

    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePhoto(int userId, int id)
    {
        // check user attempting to update there profile mathes the token the server receiving   
      if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
      {
          return Unauthorized();
      }
      var user = await _repo.GetUser(userId);

      if (!user.Photos.Any(p => p.Id == id)) // if id not match any user photo collection
      {
          return Unauthorized();
      }
      var photoFromRepo = await _repo.GetPhoto(id);
      if (photoFromRepo.IsMain) // main photo
      {
          return BadRequest("You cannot delete your main photo");
      }
      if (photoFromRepo.PublicId != null)
      {
        var deleteParams = new DeletionParams(photoFromRepo.PublicId);
        var result = _cloudinary.Destroy(deleteParams);

        if (result.Result == "ok")
        {
            _repo.Delete(photoFromRepo);
        }
      }
      if (photoFromRepo.PublicId == null)
      {
        _repo.Delete(photoFromRepo);
      }

      if (await _repo.SaveAll())
      {
          return Ok();
      }
      return BadRequest("Failed to delete the photo");
    }
  }
}