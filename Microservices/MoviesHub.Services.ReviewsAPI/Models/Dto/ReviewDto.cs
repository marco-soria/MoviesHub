namespace MoviesHub.Services.ReviewsAPI.Models.Dto
{
    public class ReviewDto
    {
        public int Id { get; set; }
        public int MovieId { get; set; }
        public string UserId { get; set; } = null!;
        public string Comment { get; set; } = null!;
        public int Rating { get; set; }
        public DateTime CreatedAt { get; set; }

        public DateTime? DeletedAt { get; set; }
    }
}
