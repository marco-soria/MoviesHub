namespace MoviesHub.Services.MoviesAPI.Models.Dto
{
    public class MovieDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int ReleaseYear { get; set; }
        public decimal AverageRating { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public List<string> GenreNames { get; set; } = new List<string>();
        public List<GenreDto> Genres { get; set; } = new List<GenreDto>();
    }
}
