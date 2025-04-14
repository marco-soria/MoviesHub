using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MoviesHub.Services.MoviesAPI.Data;
using MoviesHub.Services.MoviesAPI.Models.Dto;
using MoviesHub.Services.MoviesAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace MoviesHub.Services.MoviesAPI.Controllers
{
    [Route("api/movies")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly MovieDbContext _db;
        private readonly IMapper _mapper;
        private readonly ILogger<MoviesController> _logger;

        public MoviesController(MovieDbContext db, IMapper mapper, ILogger<MoviesController> logger)
        {
            _db = db;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<ResponseDto>> GetMovies()
        {
            var response = new ResponseDto();
            try
            {
                _logger.LogInformation("Getting all movies");
                var movies = await _db.Movies
                    .Include(m => m.MovieGenres)
                    .ThenInclude(mg => mg.Genre)
                    .ToListAsync();

                response.Result = _mapper.Map<List<MovieDto>>(movies);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all movies");
                response.IsSuccess = false;
                response.Message = "Error retrieving movies";
                response.ErrorMessages = new List<string> { ex.Message };
                return StatusCode(500, response);
            }
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ResponseDto>> GetMovie(int id)
        {
            var response = new ResponseDto();
            try
            {
                var movie = await _db.Movies
                    .Include(m => m.MovieGenres)
                    .ThenInclude(mg => mg.Genre)
                    .FirstOrDefaultAsync(m => m.Id == id);

                if (movie == null)
                {
                    _logger.LogWarning("Movie with ID: {Id} not found", id);
                    response.IsSuccess = false;
                    response.Message = $"Movie with ID: {id} not found";
                    return NotFound(response);
                }

                response.Result = _mapper.Map<MovieDto>(movie);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting movie with ID: {Id}", id);
                response.IsSuccess = false;
                response.Message = "Error retrieving movie";
                response.ErrorMessages = new List<string> { ex.Message };
                return StatusCode(500, response);
            }
        }

        [HttpPost]
        public async Task<ActionResult<ResponseDto>> CreateMovie([FromBody] MovieCreateDto movieCreateDto)
        {
            var response = new ResponseDto();
            try
            {
                if (movieCreateDto == null)
                {
                    response.IsSuccess = false;
                    response.Message = "Movie data is required";
                    return BadRequest(response);
                }

                var movie = _mapper.Map<Movie>(movieCreateDto);
                movie.CreatedAt = DateTime.UtcNow;

                await _db.Movies.AddAsync(movie);
                await _db.SaveChangesAsync();

                // Add genre relationships
                if (movieCreateDto.GenreIds != null && movieCreateDto.GenreIds.Any())
                {
                    foreach (var genreId in movieCreateDto.GenreIds)
                    {
                        if (await _db.Genres.AnyAsync(g => g.Id == genreId))
                        {
                            await _db.MovieGenres.AddAsync(new MovieGenre
                            {
                                MovieId = movie.Id,
                                GenreId = genreId
                            });
                        }
                    }
                    await _db.SaveChangesAsync();
                }

                // Get the complete movie with genres to return
                var createdMovie = await _db.Movies
                    .Include(m => m.MovieGenres)
                    .ThenInclude(mg => mg.Genre)
                    .FirstOrDefaultAsync(m => m.Id == movie.Id);

                response.Result = _mapper.Map<MovieDto>(createdMovie);

                return CreatedAtAction(nameof(GetMovie), new { id = movie.Id }, response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating movie");
                response.IsSuccess = false;
                response.Message = "Error creating movie";
                response.ErrorMessages = new List<string> { ex.Message };
                return StatusCode(500, response);
            }
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<ResponseDto>> UpdateMovie(int id, [FromBody] MovieUpdateDto movieUpdateDto)
        {
            var response = new ResponseDto();
            try
            {
                if (movieUpdateDto == null)
                {
                    response.IsSuccess = false;
                    response.Message = "Movie data is required";
                    return BadRequest(response);
                }

                var existingMovie = await _db.Movies
                    .Include(m => m.MovieGenres)
                    .FirstOrDefaultAsync(m => m.Id == id);

                if (existingMovie == null)
                {
                    _logger.LogWarning("Movie with ID: {Id} not found for update", id);
                    response.IsSuccess = false;
                    response.Message = $"Movie with ID: {id} not found";
                    return NotFound(response);
                }

                // Update movie properties
                _mapper.Map(movieUpdateDto, existingMovie);

                // Update genre relationships
                if (movieUpdateDto.GenreIds != null)
                {
                    // Remove existing genres
                    _db.MovieGenres.RemoveRange(existingMovie.MovieGenres);

                    // Add new genres
                    foreach (var genreId in movieUpdateDto.GenreIds)
                    {
                        if (await _db.Genres.AnyAsync(g => g.Id == genreId))
                        {
                            await _db.MovieGenres.AddAsync(new MovieGenre
                            {
                                MovieId = id,
                                GenreId = genreId
                            });
                        }
                    }
                }

                await _db.SaveChangesAsync();
                response.Result = true;
                response.Message = "Movie updated successfully";
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating movie with ID: {Id}", id);
                response.IsSuccess = false;
                response.Message = "Error updating movie";
                response.ErrorMessages = new List<string> { ex.Message };
                return StatusCode(500, response);
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<ResponseDto>> DeleteMovie(int id)
        {
            var response = new ResponseDto();
            try
            {
                var movie = await _db.Movies.FindAsync(id);
                if (movie == null)
                {
                    _logger.LogWarning("Movie with ID: {Id} not found for deletion", id);
                    response.IsSuccess = false;
                    response.Message = $"Movie with ID: {id} not found";
                    return NotFound(response);
                }

                // Soft delete
                movie.IsDeleted = true;
                movie.DeletedAt = DateTime.UtcNow;

                await _db.SaveChangesAsync();
                response.Result = true;
                response.Message = "Movie deleted successfully";
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting movie with ID: {Id}", id);
                response.IsSuccess = false;
                response.Message = "Error deleting movie";
                response.ErrorMessages = new List<string> { ex.Message };
                return StatusCode(500, response);
            }
        }

        // Additional endpoint to restore a soft-deleted movie
        [HttpPatch("{id:int}/restore")]
        public async Task<ActionResult<ResponseDto>> RestoreMovie(int id)
        {
            var response = new ResponseDto();
            try
            {
                var movie = await _db.Movies
                    .IgnoreQueryFilters() // Important to find soft-deleted movies
                    .FirstOrDefaultAsync(m => m.Id == id && m.IsDeleted);

                if (movie == null)
                {
                    response.IsSuccess = false;
                    response.Message = $"Deleted movie with ID: {id} not found";
                    return NotFound(response);
                }

                movie.IsDeleted = false;
                movie.DeletedAt = null;

                await _db.SaveChangesAsync();
                response.Result = true;
                response.Message = "Movie restored successfully";
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error restoring movie with ID: {Id}", id);
                response.IsSuccess = false;
                response.Message = "Error restoring movie";
                response.ErrorMessages = new List<string> { ex.Message };
                return StatusCode(500, response);
            }
        }

        // Get movies by genre
        [HttpGet("bygenre/{genreId:int}")]
        public async Task<ActionResult<ResponseDto>> GetMoviesByGenre(int genreId)
        {
            var response = new ResponseDto();
            try
            {
                var movies = await _db.Movies
                    .Include(m => m.MovieGenres)
                    .ThenInclude(mg => mg.Genre)
                    .Where(m => m.MovieGenres.Any(mg => mg.GenreId == genreId))
                    .ToListAsync();

                response.Result = _mapper.Map<List<MovieDto>>(movies);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting movies for genre with ID: {Id}", genreId);
                response.IsSuccess = false;
                response.Message = "Error retrieving movies by genre";
                response.ErrorMessages = new List<string> { ex.Message };
                return StatusCode(500, response);
            }
        }

        [HttpGet("deleted")]
        public async Task<ActionResult<ResponseDto>> GetDeletedMovies()
        {
            var response = new ResponseDto();
            try
            {
                var deletedMovies = await _db.Movies
                    .IgnoreQueryFilters() // Ignore the global query filter for IsDeleted
                    .Where(m => m.IsDeleted)
                    .Include(m => m.MovieGenres)
                    .ThenInclude(mg => mg.Genre)
                    .ToListAsync();

                response.Result = _mapper.Map<List<MovieDto>>(deletedMovies);
                response.IsSuccess = true;
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving deleted movies");
                response.IsSuccess = false;
                response.Message = "Error retrieving deleted movies";
                response.ErrorMessages = new List<string> { ex.Message };
                return StatusCode(500, response);
            }
        }
    }
}
