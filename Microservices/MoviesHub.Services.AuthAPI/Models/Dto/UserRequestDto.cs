using System.ComponentModel.DataAnnotations;

namespace MoviesHub.Services.AuthAPI.Models.Dto
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
