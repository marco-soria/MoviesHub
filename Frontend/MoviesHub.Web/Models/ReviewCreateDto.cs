using System.ComponentModel.DataAnnotations;

namespace MoviesHub.Web.Models
{
    public class ReviewCreateDto
    {
        [Required]
        public int MovieId { get; set; }

        [Required]
        public string UserId { get; set; } = null!;

        [Required]
        [StringLength(1000, MinimumLength = 5, ErrorMessage = "El comentario debe tener entre 5 y 1000 caracteres")]
        public string Comment { get; set; } = null!;

        [Required]
        [Range(1, 10, ErrorMessage = "La calificación debe estar entre 1 y 10")]
        public int Rating { get; set; }
    }
}
