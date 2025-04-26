namespace MoviesHub.Web.Models
{
    public class MovieDetailsViewModel
    {
        public MovieDto Movie { get; set; }
        public List<ReviewDto> Reviews { get; set; } = new List<ReviewDto>();
        public ReviewCreateDto NewReview { get; set; } = new ReviewCreateDto();

        // Rating calculado directamente de las reviews
        public decimal CalculatedAverageRating { get; set; }

        // El usuario tiene un rating para esta película?
        public bool HasUserRating => UserRating > 0;

        // El rating del usuario actual (si existe)
        public int UserRating { get; set; }
    }
}
