namespace MoviesHub.Web.Models
{
    public class RegistrationRequestDto
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? PhoneNumber { get; set; }
        public string Password { get; set; }
        public string? Role { get; set; }
    }
}
