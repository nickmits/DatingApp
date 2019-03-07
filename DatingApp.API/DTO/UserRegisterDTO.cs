using System;
using System.ComponentModel.DataAnnotations;

namespace DatingApp.API.DTO
{
    public class UserRegisterDTO
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [StringLength(15, MinimumLength = 5, ErrorMessage = "Password must be between 5 - 15 characters")]
        public string Password { get; set; }

        [Required]
        public string Gender { get; set; }

        [Required]
        public string knownAs { get; set; }

        [Required]
        public DateTime DateOfBith { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public string Country { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastActive { get; set; }
        public UserRegisterDTO()
        {
            Created = DateTime.Now;
            LastActive = DateTime.Now;
        }   
    }
}