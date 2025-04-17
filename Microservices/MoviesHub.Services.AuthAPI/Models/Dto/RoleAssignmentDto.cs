using System.ComponentModel.DataAnnotations;

namespace MoviesHub.Services.AuthAPI.Models.Dto
{
    public class RoleAssignmentDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Role { get; set; }
    }
}
