using System.ComponentModel.DataAnnotations.Schema;

namespace MoviesHub.Services.MoviesAPI.Models
{
    public class Movie
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public int ReleaseYear { get; set; }
        public decimal AverageRating { get; set; } = 0;
        public string ImageUrl { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Campos para Soft Delete
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }

        // Relación con géneros 
        public ICollection<MovieGenre> MovieGenres { get; set; } = new List<MovieGenre>();

        // Propiedad calculada
        [NotMapped]
        public List<string> GenreNames => MovieGenres?
            .Where(mg => mg.Genre != null && !string.IsNullOrEmpty(mg.Genre.Name))
            .Select(mg => mg.Genre!.Name!)
            .ToList() ?? new List<string>();
    }
}
