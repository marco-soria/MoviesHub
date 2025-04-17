using System.ComponentModel.DataAnnotations;

namespace MoviesHub.Web.Models
{
    public class RegistrationRequestDto
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }

        public string? PhoneNumber { get; set; }
        [Required]
        public string Password { get; set; }
        public string? Role { get; set; }
    }
}
