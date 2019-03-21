using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.API.Data;
using DatingApp.API.DTO;
using DatingApp.API.Helpers;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.API.Controllers
{
    [ServiceFilter(typeof(LogUserActivity))]
    [Authorize]
    [Route("api/users/{userId}[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly IDatingRepository rep;
        private readonly IMapper map;

        public MessagesController(IDatingRepository repository, IMapper mapper)
        {
            rep = repository;
            map = mapper;           
        }

        [HttpGet("{id}", Name = "GetMessage")]
        public async Task<IActionResult> GetMessage(int userId, int id)
        {
            if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var messageFromRepo = await rep.GetMessage(id);

            if(messageFromRepo == null)
                return NotFound();

            return Ok(messageFromRepo);        
        }

        [HttpGet]
        public async Task<IActionResult> GetMessagesForUser(int userId, [FromQuery]MessageParams messageParams)
        {
            if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            messageParams.UserId = userId;    

            var messageFromRepo = await rep.GetMessagesForUser(messageParams);

            Response.AddPagination(messageFromRepo.CurrentPage, messageFromRepo.PageSize,
                 messageFromRepo.TotalCount, messageFromRepo.TotalPages);

            return Ok(map.Map<IEnumerable<MessageToReturnDto>>(messageFromRepo));
        }

        [HttpGet("thread/{recipientId")]
        public async Task<IActionResult> GetMessageThread(int userId,int recipientId)
        {
            if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var messageFromRepo = await rep.GetMessageThread(userId, recipientId);
             
            return Ok(map.Map<IEnumerable<MessageToReturnDto>>(messageFromRepo));
        }

        [HttpPost]
        public async Task<IActionResult> CreateMessage(int userId, MessageForCreationDto messageForCreationDto)
        {
            var sender = await rep.GetUser(userId);

            if(sender.Id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            messageForCreationDto.SenderId = userId;

            var recipient = await rep.GetUser(messageForCreationDto.RecipientId);

            if (recipient == null)
                return BadRequest("Could not found user");

            var message = map.Map<Message>(messageForCreationDto);           

            rep.Add(message);           

            if (await rep.SaveAll()) {
                 var messageToReturn = map.Map<MessageToReturnDto>(message);
                return CreatedAtRoute("GetMessage", new Message{Id = message.Id}, messageToReturn);
            }                

            throw new Exception("Creating the message failed on save");    
        }

        [HttpPost("{id}")]
        public async Task<IActionResult> DeleteMessage(int id, int userId)
        {
            if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var messageFromRepo = await rep.GetMessage(id);

            if(messageFromRepo.SenderId == userId)
                messageFromRepo.SenderDeleted = true;

            if(messageFromRepo.RecipientId == userId)
                messageFromRepo.RecipientDeleted = true;

            if(messageFromRepo.SenderDeleted && messageFromRepo.RecipientDeleted)
                rep.Delete(messageFromRepo);

            if(await rep.SaveAll())
                return NoContent();

            throw new Exception("Error deleting the message");                       
        }

        [HttpPost("{id}/read")]
        public async Task<IActionResult> MarkMessageAsRead(int userId, int id)
        {
            if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var message = await rep.GetMessage(id);

            if(message.RecipientId != userId)
                return Unauthorized();

            message.IsRead = true;
            message.DateRead = DateTime.Now;

            await rep.SaveAll();

            return NoContent();          
        }
    }
}