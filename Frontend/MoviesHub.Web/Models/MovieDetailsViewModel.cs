namespace MoviesHub.Web.Models
{
    public class MovieDetailsViewModel
    {
        public MovieDto Movie { get; set; }
        public List<ReviewDto> Reviews { get; set; } = new List<ReviewDto>();
        public ReviewCreateDto NewReview { get; set; } = new ReviewCreateDto();
    }
}
