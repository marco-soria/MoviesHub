namespace MoviesHub.Services.AuthAPI.Models.Dto
{
    public class UserWithRoleDto
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public List<string> Roles { get; set; } = new List<string>();
    }
}
