namespace MoviesHub.Web.Models
{
    public class ReviewDto
    {
        public int Id { get; set; }
        public int MovieId { get; set; }
        public string UserId { get; set; } = null!;
        public string Comment { get; set; } = null!;
        public int Rating { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
        //Added
        public string UserName { get; set; }
    }
}
