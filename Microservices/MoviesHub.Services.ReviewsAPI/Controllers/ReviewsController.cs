using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesHub.Services.ReviewsAPI.Data;
using MoviesHub.Services.ReviewsAPI.Models;
using MoviesHub.Services.ReviewsAPI.Models.Dto;
using MoviesHub.Services.ReviewsAPI.Services.IServices;

namespace MoviesHub.Services.ReviewsAPI.Controllers
{
    [Route("api/reviews")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly ReviewDbContext _db;
        private readonly IMapper _mapper;
        private readonly ILogger<ReviewsController> _logger;
        private readonly IMovieAPIService _movieService;
        private readonly IHttpClientFactory _httpClientFactory; // Para notificaciones

        public ReviewsController(
            ReviewDbContext db,
            IMapper mapper,
            ILogger<ReviewsController> logger,
            IMovieAPIService movieService,
            IHttpClientFactory httpClientFactory)
        {
            _db = db;
            _mapper = mapper;
            _logger = logger;
            _movieService = movieService;
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public async Task<ActionResult<ResponseDto>> GetReviews()
        {
            var response = new ResponseDto();
            try
            {
                _logger.LogInformation("Getting all reviews");
                var reviews = await _db.Reviews.ToListAsync();
                response.Result = _mapper.Map<List<ReviewDto>>(reviews);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all reviews");
                response.IsSuccess = false;
                response.Message = "Error retrieving reviews";
                response.ErrorMessages.Add(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ResponseDto>> GetReviewById(int id)
        {
            var response = new ResponseDto();
            try
            {
                var review = await _db.Reviews.FindAsync(id);
                if (review == null)
                {
                    _logger.LogWarning("Review with ID: {Id} not found", id);
                    response.IsSuccess = false;
                    response.Message = "Review not found";
                    return NotFound(response);
                }

                response.Result = _mapper.Map<ReviewDto>(review);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting review with ID: {Id}", id);
                response.IsSuccess = false;
                response.Message = "Error retrieving review";
                response.ErrorMessages.Add(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }

        [HttpPost]
        public async Task<ActionResult<ResponseDto>> CreateReview([FromBody] ReviewCreateDto reviewCreateDto)
        {
            var response = new ResponseDto();
            try
            {
                if (reviewCreateDto == null)
                {
                    response.IsSuccess = false;
                    response.Message = "Invalid data provided";
                    return BadRequest(response);
                }

                // Verificar si el usuario ya ha revisado esta película (incluyendo las eliminadas)
                var existingReview = await _db.Reviews
                    .IgnoreQueryFilters() // Incluir reseñas eliminadas
                    .FirstOrDefaultAsync(r => r.MovieId == reviewCreateDto.MovieId && r.UserId == reviewCreateDto.UserId);

                if (existingReview != null)
                {
                    if (!existingReview.IsDeleted)
                    {
                        // La reseña existe y no está borrada
                        response.IsSuccess = false;
                        response.Message = "User has already reviewed this movie";
                        return Conflict(response);
                    }
                    else
                    {
                        // La reseña existe pero está borrada, podemos restaurarla con los nuevos datos
                        existingReview.Comment = reviewCreateDto.Comment;
                        existingReview.Rating = reviewCreateDto.Rating;
                        existingReview.IsDeleted = false;
                        existingReview.DeletedAt = null;
                        existingReview.CreatedAt = DateTime.UtcNow; // Opcional: actualizar la fecha

                        await _db.SaveChangesAsync();

                        // Notificar a MoviesAPI sobre el cambio en calificaciones
                        await NotifyMoviesAPIOfRatingChange(reviewCreateDto.MovieId);

                        response.Result = _mapper.Map<ReviewDto>(existingReview);
                        response.Message = "Review restored and updated successfully";
                        return CreatedAtAction(nameof(GetReviewById), new { id = existingReview.Id }, response);
                    }
                }

                // Si no existe una reseña para esta combinación de usuario y película
                var review = _mapper.Map<Review>(reviewCreateDto);
                await _db.Reviews.AddAsync(review);
                await _db.SaveChangesAsync();

                // Notificar a MoviesAPI sobre el cambio en calificaciones
                await NotifyMoviesAPIOfRatingChange(reviewCreateDto.MovieId);

                response.Result = _mapper.Map<ReviewDto>(review);
                response.Message = "Review created successfully";

                return CreatedAtAction(nameof(GetReviewById), new { id = review.Id }, response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating review: {Message}", ex.Message);
                response.IsSuccess = false;
                response.Message = "Error creating review";
                response.ErrorMessages.Add(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }



        [HttpPut("{id:int}")]
        public async Task<ActionResult<ResponseDto>> UpdateReview(int id, [FromBody] ReviewUpdateDto reviewUpdateDto)
        {
            var response = new ResponseDto();
            try
            {
                if (reviewUpdateDto == null)
                {
                    response.IsSuccess = false;
                    response.Message = "Invalid data provided";
                    return BadRequest(response);
                }

                var review = await _db.Reviews.FindAsync(id);
                if (review == null)
                {
                    _logger.LogWarning("Review with ID: {Id} not found for update", id);
                    response.IsSuccess = false;
                    response.Message = "Review not found";
                    return NotFound(response);
                }

                _mapper.Map(reviewUpdateDto, review);
                await _db.SaveChangesAsync();

                // Notificar a MoviesAPI sobre el cambio en calificaciones
                await NotifyMoviesAPIOfRatingChange(review.MovieId);

                response.Message = "Review updated successfully";
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating review with ID: {Id}", id);
                response.IsSuccess = false;
                response.Message = "Error updating review";
                response.ErrorMessages.Add(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }


        [HttpDelete("{id:int}")]
        public async Task<ActionResult<ResponseDto>> DeleteReview(int id)
        {
            var response = new ResponseDto();
            try
            {
                var review = await _db.Reviews.FindAsync(id);
                if (review == null)
                {
                    _logger.LogWarning("Review with ID: {Id} not found for deletion", id);
                    response.IsSuccess = false;
                    response.Message = "Review not found";
                    return NotFound(response);
                }

                int movieId = review.MovieId; // Guardar el movieId antes del soft delete

                // Soft delete
                review.IsDeleted = true;
                review.DeletedAt = DateTime.UtcNow;
                await _db.SaveChangesAsync();

                // Notificar a MoviesAPI sobre el cambio en calificaciones
                await NotifyMoviesAPIOfRatingChange(movieId);

                response.Message = "Review deleted successfully";
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting review with ID: {Id}", id);
                response.IsSuccess = false;
                response.Message = "Error deleting review";
                response.ErrorMessages.Add(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }


        // Additional endpoint to restore a soft-deleted review
        [HttpPatch("{id:int}/restore")]
        public async Task<ActionResult<ResponseDto>> RestoreReview(int id)
        {
            var response = new ResponseDto();
            try
            {
                var review = await _db.Reviews
                    .IgnoreQueryFilters() // Important to find soft-deleted reviews
                    .FirstOrDefaultAsync(r => r.Id == id && r.IsDeleted);

                if (review == null)
                {
                    response.IsSuccess = false;
                    response.Message = "Deleted review not found";
                    return NotFound(response);
                }

                review.IsDeleted = false;
                review.DeletedAt = null;
                await _db.SaveChangesAsync();

                // Notificar a MoviesAPI sobre el cambio en calificaciones
                await NotifyMoviesAPIOfRatingChange(review.MovieId);

                response.Message = "Review restored successfully";
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error restoring review with ID: {Id}", id);
                response.IsSuccess = false;
                response.Message = "Error restoring review";
                response.ErrorMessages.Add(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }


        // Get reviews by movie ID
        [HttpGet("movie/{movieId:int}")]
        public async Task<ActionResult<ResponseDto>> GetReviewsByMovie(int movieId)
        {
            var response = new ResponseDto();
            try
            {
                var reviews = await _db.Reviews.Where(r => r.MovieId == movieId).ToListAsync();
                response.Result = _mapper.Map<List<ReviewDto>>(reviews);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting reviews for movie with ID: {Id}", movieId);
                response.IsSuccess = false;
                response.Message = "Error retrieving reviews for movie";
                response.ErrorMessages.Add(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }

        // Get reviews by user ID
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<ResponseDto>> GetReviewsByUser(string userId)
        {
            var response = new ResponseDto();
            try
            {
                var reviews = await _db.Reviews.Where(r => r.UserId == userId).ToListAsync();
                response.Result = _mapper.Map<List<ReviewDto>>(reviews);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting reviews for user with ID: {Id}", userId);
                response.IsSuccess = false;
                response.Message = "Error retrieving reviews for user";
                response.ErrorMessages.Add(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }

        // Get reviews by rating
        //[HttpGet("rating/{rating:int}")]
        //public async Task<ActionResult<ResponseDto>> GetReviewsByRating(int rating)
        //{
        //    var response = new ResponseDto();
        //    try
        //    {
        //        if (rating < 1 || rating > 10)
        //        {
        //            response.IsSuccess = false;
        //            response.Message = "Rating must be between 1 and 10";
        //            return BadRequest(response);
        //        }

        //        var reviews = await _db.Reviews.Where(r => r.Rating == rating).ToListAsync();
        //        response.Result = _mapper.Map<List<ReviewDto>>(reviews);
        //        return Ok(response);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error getting reviews with rating: {Rating}", rating);
        //        response.IsSuccess = false;
        //        response.Message = "Error retrieving reviews by rating";
        //        response.ErrorMessages.Add(ex.Message);
        //        return StatusCode(StatusCodes.Status500InternalServerError, response);
        //    }
        //}

        // Get all deleted reviews (for admin purposes)
        [HttpGet("deleted")]
        public async Task<ActionResult<ResponseDto>> GetDeletedReviews()
        {
            var response = new ResponseDto();
            try
            {
                var deletedReviews = await _db.Reviews
                    .IgnoreQueryFilters() // Ignore the global query filter for IsDeleted
                    .Where(r => r.IsDeleted)
                    .ToListAsync();

                response.Result = _mapper.Map<List<ReviewDto>>(deletedReviews);
                response.IsSuccess = true;
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving deleted reviews");
                response.IsSuccess = false;
                response.Message = "Error retrieving deleted reviews";
                response.ErrorMessages.Add(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }

        // En ReviewsController.cs de ReviewsAPI
        [HttpGet("movie/{movieId:int}/average")]
        public async Task<ActionResult<ResponseDto>> GetAverageRatingForMovie(int movieId)
        {
            var response = new ResponseDto();
            try
            {
                _logger.LogInformation("Calculating average rating for movie ID: {MovieId}", movieId);

                // Verificar si la película existe mediante el servicio de MovieAPI
                bool movieExists = await _movieService.MovieExistsAsync(movieId);
                if (!movieExists)
                {
                    _logger.LogWarning("Attempted to get average rating for non-existent movie ID: {MovieId}", movieId);
                    response.IsSuccess = false;
                    response.Message = "Movie not found";
                    return NotFound(response);
                }

                // Obtener todas las reviews no eliminadas para esta película
                var reviews = await _db.Reviews
                    .Where(r => r.MovieId == movieId && !r.IsDeleted)
                    .ToListAsync();

                // Manejar el caso cuando no hay reviews (evitar DivideByZeroException)
                if (reviews == null || !reviews.Any())
                {
                    _logger.LogInformation("No reviews found for movie ID: {MovieId}, returning 0", movieId);
                    response.Result = 0.0;
                    return Ok(response);
                }

                // Calcular el promedio de forma segura
                double average = reviews.Average(r => (double)r.Rating);

                // Redondear a 2 decimales
                average = Math.Round(average, 2);

                _logger.LogInformation("Average rating for movie ID: {MovieId} is {Rating}", movieId, average);
                response.Result = average;
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating average rating for movie {MovieId}", movieId);
                response.IsSuccess = false;
                response.Message = "Error calculating average rating";
                response.ErrorMessages.Add(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }

        private async Task NotifyMoviesAPIOfRatingChange(int movieId)
        {
            try
            {
                _logger.LogInformation("Notifying MoviesAPI about rating change for movie ID: {MovieId}", movieId);

                // Usar directamente IMovieAPIService para notificar
                bool success = await _movieService.NotifyRatingChangeAsync(movieId);

                if (!success)
                {
                    _logger.LogWarning("Failed to notify MoviesAPI about rating change for movie ID: {MovieId}", movieId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error notifying MoviesAPI about rating change for movie {MovieId}", movieId);
                // No lanzamos la excepción para no afectar el flujo principal
            }
        }


    }
}
