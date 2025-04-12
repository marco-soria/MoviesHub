using System.ComponentModel.DataAnnotations;

namespace MoviesHub.Services.ReviewsAPI.Models
{
    public class Review
    {
        public int Id { get; set; }
        public int MovieId { get; set; } // FK a Movies.API
        public string UserId { get; set; } = null!; // FK a Auth.API
        public string Comment { get; set; } = null!;
        public int Rating { get; set; } 
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Campos para Soft Delete
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }
    }
}
