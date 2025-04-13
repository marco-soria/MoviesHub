namespace MoviesHub.Web.Models
{
    public class MovieGenreDto
    {
        public int MovieId { get; set; }
        public int GenreId { get; set; }
        public string MovieTitle { get; set; } = string.Empty;
        public string GenreName { get; set; } = string.Empty;
    }
}
