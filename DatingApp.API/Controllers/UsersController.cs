using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.API.Data;
using DatingApp.API.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IDatingRepository rep;

        public IMapper Map { get; }

        public UsersController(IDatingRepository repo, IMapper mapper)
        {
            rep = repo;
            Map = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            return Ok(Map.Map<IEnumerable<UserForListDTO>>
            (await rep.GetUsers()));//user to return

        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
           return Ok(Map.Map<UserForDetailedDTO>//user to return
           (await rep.GetUser(id)));
            
        }
    }
}