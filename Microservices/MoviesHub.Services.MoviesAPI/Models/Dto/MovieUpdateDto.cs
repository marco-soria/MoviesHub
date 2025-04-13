namespace MoviesHub.Services.MoviesAPI.Models.Dto
{
    public class MovieUpdateDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int ReleaseYear { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public List<int> GenreIds { get; set; } = new List<int>();
    }
}
