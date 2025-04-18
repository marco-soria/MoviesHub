using System.ComponentModel.DataAnnotations;

namespace MoviesHub.Web.Models
{
    public class UserRequestDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        public string FirstName { get; set; } = null!;

        [Required]
        public string LastName { get; set; } = null!;

        public string? PhoneNumber { get; set; }

        [MinLength(6)]
        public string? Password { get; set; }

        public string? Role { get; set; }
    }
}
