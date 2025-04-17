using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesHub.Services.ReviewsAPI.Data;
using MoviesHub.Services.ReviewsAPI.Models;
using MoviesHub.Services.ReviewsAPI.Models.Dto;

namespace MoviesHub.Services.ReviewsAPI.Controllers
{
    [Route("api/reviews")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly ReviewDbContext _db;
        private readonly IMapper _mapper;
        private readonly ILogger<ReviewsController> _logger;

        public ReviewsController(ReviewDbContext db, IMapper mapper, ILogger<ReviewsController> logger)
        {
            _db = db;
            _mapper = mapper;
            _logger = logger;
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

                // Verificar si el usuario ya ha revisado esta película
                if (await _db.Reviews.AnyAsync(r => r.MovieId == reviewCreateDto.MovieId && r.UserId == reviewCreateDto.UserId))
                {
                    response.IsSuccess = false;
                    response.Message = "User has already reviewed this movie";
                    return Conflict(response);
                }

                var review = _mapper.Map<Review>(reviewCreateDto);
                await _db.Reviews.AddAsync(review);
                await _db.SaveChangesAsync();

                response.Result = _mapper.Map<ReviewDto>(review);
                response.Message = "Review created successfully";

                return CreatedAtAction(nameof(GetReviewById), new { id = review.Id }, response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating review");
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

                // Soft delete
                review.IsDeleted = true;
                review.DeletedAt = DateTime.UtcNow;
                await _db.SaveChangesAsync();

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
    }
}
