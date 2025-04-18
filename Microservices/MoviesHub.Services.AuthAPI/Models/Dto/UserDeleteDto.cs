namespace MoviesHub.Services.AuthAPI.Models.Dto
{
    public class UserDeleteDto
    {
        public string Id { get; set; } = null!;
        public bool Permanent { get; set; } = false;
    }
}
