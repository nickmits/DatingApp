using System.ComponentModel.DataAnnotations;
namespace DatingApp.API.DTO
{
    public class UserRegisterDTO
    {
        [Required]
        [StringLength(15, MinimumLength = 5, ErrorMessage = "Username must be between 5 - 15 characters")]
        public string Username { get; set; }

        [Required]
        [StringLength(15, MinimumLength = 5, ErrorMessage = "Password must be between 5 - 15 characters")]
        public string Password { get; set; }
    }
}