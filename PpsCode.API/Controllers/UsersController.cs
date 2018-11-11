using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PpsCode.API.Data;
using PpsCode.API.Dtos;

namespace PpsCode.API.Controllers
{
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

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUser(int id)
    {
      var user = await _repo.GetUser(id);

      // execute mapping from the source obj to destination obj
      var userToReturn = _mapper.Map<UserForDetailedDto>(user);

      return Ok(userToReturn);
    }
  }
}