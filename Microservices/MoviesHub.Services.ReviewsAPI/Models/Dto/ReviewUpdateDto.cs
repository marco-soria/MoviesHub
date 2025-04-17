using System.ComponentModel.DataAnnotations;

namespace MoviesHub.Services.ReviewsAPI.Models.Dto
{
    public class ReviewUpdateDto
    {
        [Required]
        [StringLength(1000, MinimumLength = 5, ErrorMessage = "El comentario debe tener entre 5 y 1000 caracteres")]
        public string Comment { get; set; } = null!;

        [Required]
        [Range(1, 10, ErrorMessage = "La calificación debe estar entre 1 y 10")]
        public int Rating { get; set; }
    }
}
