using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PpsCode.API.Data;
using PpsCode.API.Dtos;
using PpsCode.API.Models;

namespace PpsCode.API.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class AuthController : ControllerBase
  {
    private readonly IAuthRepository _repo;
    private readonly IConfiguration _config;
    private readonly IMapper _mapper;
    public AuthController(IAuthRepository repo, IConfiguration config, IMapper mapper)
    {
      _mapper = mapper;
      _config = config;
      _repo = repo;

    }

    [HttpPost("register")]
    // DTO(data transfer obj)
    public async Task<IActionResult> Register(UserForRegisterDto userForRegisterDto)
    {
      // validate request on UserForRegisterDto.cs

      // send username then convert to lowercase
      userForRegisterDto.Username = userForRegisterDto.Username.ToLower();

      // check if the username is taken
      if (await _repo.UserExists(userForRegisterDto.Username))
      {
        return BadRequest("Username already exists");
      }

      var userToCreate = new User
      {
        Username = userForRegisterDto.Username
      };

      var createdUser = await _repo.Register(userToCreate, userForRegisterDto.Password);

      return StatusCode(201);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(UserForLoginDto userForLoginDto)
    {
      // - return token to users once they loggedIn 
      // - then user be able to use the token to  Auth against the API 

      // check if user exist
      var userFromRepo = await _repo.Login(userForLoginDto.Username.ToLower(), userForLoginDto.Password);

      // check if there is anything inside from _repo
      if (userFromRepo == null)
      {
        return Unauthorized();
      }

      var claims = new[] // 
      {
            new Claim(ClaimTypes.NameIdentifier, userFromRepo.Id.ToString()),
            new Claim(ClaimTypes.Name, userFromRepo.Username)
        };

      // creating security key and use z key and encrypt
      var key = new SymmetricSecurityKey(Encoding.UTF8
      .GetBytes(_config.GetSection("AppSettings:Token").Value)); // appsettings.json

      var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
      // security token descriptor (contain claims and expiry of token & signing credentials) 
      var tokenDescriptor = new SecurityTokenDescriptor
      {
        Subject = new ClaimsIdentity(claims),
        Expires = DateTime.Now.AddDays(1),
        SigningCredentials = creds
      };
      // token handler

      var tokenHandler = new JwtSecurityTokenHandler();

      // using tokenHandlet create token and pass token discriptor

      var token = tokenHandler.CreateToken(tokenDescriptor); // contain JWT to return

      var user = _mapper.Map<UserForListDto>(userFromRepo);

        // return JWT token to client
      return Ok(new
        {
          token = tokenHandler.WriteToken(token),
          user
        });
    }
  }
}