using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.API.Data;
using DatingApp.API.DTO;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DatingApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository repo;
        private readonly IConfiguration config;
        private readonly IMapper _mapper;

        public AuthController(IAuthRepository repository, IConfiguration configuration, IMapper mapper)
        {
            repo = repository;
            config = configuration;
            _mapper = mapper;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegisterDTO UserData)
        {
            UserData.Username = UserData.Username.ToLower();

            if(await repo.UserExists(UserData.Username))
                return BadRequest("Username already exists");

            var userToCreate = _mapper.Map<User>(UserData);

            User NewUser = await repo.Register(new User{ Username = UserData.Username}, UserData.Password);
            
            var userToReturn = _mapper.Map<UserForDetailedDTO>(NewUser);

            return CreatedAtRoute("GetUser", new {controller = "Users", id = NewUser.Id}, userToReturn); 
        
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginDTO UserData)
        {
            User LoggedIn = await repo.Login(UserData.Username.ToLower(), UserData.Password);

            if(LoggedIn == null) return Unauthorized();
        // User exists so we create token

            var claims = new[]{
                new Claim(ClaimTypes.NameIdentifier, LoggedIn.Id.ToString()),
                new Claim(ClaimTypes.Name, LoggedIn.Username)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8
            .GetBytes(config.GetSection("TokenSettings:Token").Value));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var TokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddHours(1),
                SigningCredentials = credentials
            };
            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.WriteToken(tokenHandler.CreateToken(TokenDescriptor));

            var user = _mapper.Map<UserForListDTO>(LoggedIn);

            return Ok(new {
                token,
                user 
            });
        }
    }
}