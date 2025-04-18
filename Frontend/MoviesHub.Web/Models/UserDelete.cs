namespace MoviesHub.Web.Models
{
    public class UserDeleteDto
    {
        public string Id { get; set; } = null!;
        public bool Permanent { get; set; } = false;
    }
}
