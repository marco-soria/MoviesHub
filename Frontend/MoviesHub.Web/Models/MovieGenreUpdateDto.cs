namespace MoviesHub.Web.Models
{
    public class MovieGenreUpdateDto
    {
        public int MovieId { get; set; }
        public int GenreId { get; set; }
        public int NewGenreId
        {
            get; set;
        }
    }
}
