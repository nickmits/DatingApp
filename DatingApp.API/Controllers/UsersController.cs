using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.API.Data;
using DatingApp.API.DTO;
using DatingApp.API.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.API.Controllers
{
    [ServiceFilter(typeof(LogUserActivity))]
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

        [HttpGet("{id}", Name = "GetUser")]
        public async Task<IActionResult> GetUser(int id)
        {
           return Ok(Map.Map<UserForDetailedDTO>//user to return
           (await rep.GetUser(id)));            
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateUser(int id, UserForUpdateDto userForUpdateDto)
        {
            if(id !=int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var userFromRepo = await rep.GetUser(id);

            Map.Map(userForUpdateDto, userFromRepo) ;

            if(await rep.SaveAll()) return NoContent();

            throw new Exception($"updating user {id} failed on save"); 
        }
    }
}