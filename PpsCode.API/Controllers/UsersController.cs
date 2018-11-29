using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PpsCode.API.Data;
using PpsCode.API.Dtos;
using PpsCode.API.Helpers;

namespace PpsCode.API.Controllers
{
  [ServiceFilter(typeof(LogUserActivity))]
  [Authorize]
  [Route("api/[controller]")]
  [ApiController]
  public class UsersController : ControllerBase
  {
    private readonly IDevRepository _repo;
    private readonly IMapper _mapper;
    public UsersController(IDevRepository repo, IMapper mapper)
    {
      _mapper = mapper;
      _repo = repo;

    }

    [HttpGet]
    public async Task<IActionResult> GetUsers()
    {
      var users = await _repo.GetUsers();

      var usersToReturn = _mapper.Map<IEnumerable<UserForListDto>>(users);

      return Ok(usersToReturn);
    }

    [HttpGet("{id}", Name = "GetUser")]
    public async Task<IActionResult> GetUser(int id)
    {
      var user = await _repo.GetUser(id);

      // execute mapping from the source obj to destination obj
      var userToReturn = _mapper.Map<UserForDetailedDto>(user);

      return Ok(userToReturn);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(int id, UserForUpdateDto userForUpdateDto)
    {
      // check user attempting to update there profile mathes the token the server receiving   
      if (id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
      {
          return Unauthorized();

      } 
      var userFromRepo = await _repo.GetUser(id);
      // updates the values userForUpdateDto and write them into userFromRepo
      _mapper.Map(userForUpdateDto, userFromRepo);

      if (await _repo.SaveAll())
      {
          return NoContent();
      }
      throw new Exception($"Updating user {id} failed on save");
    }
  }
}