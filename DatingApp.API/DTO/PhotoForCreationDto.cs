using System;
using Microsoft.AspNetCore.Http;

namespace DatingApp.API.DTO
{
    public class PhotoForCreationDto
    {
        public string Url { get; set; }
        public IFormFile File { get; set; }
        public string Descriptiom { get; set; }
        public DateTime DateAdded { get; set; }
        public string PublicId { get; set; }

        public PhotoForCreationDto()
        {
            DateAdded = DateTime.Now;
        }
    }
}