using System.ComponentModel.DataAnnotations;

namespace MoviesHub.Web.Models
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
