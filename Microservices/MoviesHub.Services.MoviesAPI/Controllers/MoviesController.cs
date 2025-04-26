using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MoviesHub.Services.MoviesAPI.Data;
using MoviesHub.Services.MoviesAPI.Models.Dto;
using MoviesHub.Services.MoviesAPI.Models;
using Microsoft.EntityFrameworkCore;
using MoviesHub.Services.MoviesAPI.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using MoviesHub.Services.MoviesAPI.Services;


namespace MoviesHub.Services.MoviesAPI.Controllers
{
    [Route("api/movies")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly MovieDbContext _db;
        private readonly IMapper _mapper;
        private readonly ILogger<MoviesController> _logger;
        private readonly IReviewAPIService _reviewService;

        public MoviesController(
            MovieDbContext db,
            IMapper mapper,
            ILogger<MoviesController> logger,
            IReviewAPIService reviewService)
        {
            _db = db;
            _mapper = mapper;
            _logger = logger;
            _reviewService = reviewService;
        }

        //[HttpGet]
        //public async Task<ActionResult<ResponseDto>> GetMovies()
        //{
        //    var response = new ResponseDto();
        //    try
        //    {
        //        _logger.LogInformation("Getting all movies");
        //        var movies = await _db.Movies
        //            .Include(m => m.MovieGenres)
        //            .ThenInclude(mg => mg.Genre)
        //            .ToListAsync();

        //        response.Result = _mapper.Map<List<MovieDto>>(movies);
        //        return Ok(response);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error getting all movies");
        //        response.IsSuccess = false;
        //        response.Message = "Error retrieving movies";
        //        response.ErrorMessages = new List<string> { ex.Message };
        //        return StatusCode(500, response);
        //    }
        //}
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

                // No actualizamos las calificaciones en este caso para no ralentizar la API
                // Las calificaciones se actualizarán bajo demanda cuando se acceda a películas individuales

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


        // En MoviesController.cs del servicio MoviesAPI

        //[HttpGet("{id:int}")]
        //public async Task<ActionResult<ResponseDto>> GetMovie(int id)
        //{
        //    var response = new ResponseDto();
        //    try
        //    {
        //        var movie = await _db.Movies
        //            .Include(m => m.MovieGenres)
        //            .ThenInclude(mg => mg.Genre)
        //            .FirstOrDefaultAsync(m => m.Id == id);

        //        if (movie == null)
        //        {
        //            _logger.LogWarning("Movie with ID: {Id} not found", id);
        //            response.IsSuccess = false;
        //            response.Message = $"Movie with ID: {id} not found";
        //            return NotFound(response);
        //        }

        //        // Actualizar la calificación promedio desde ReviewsAPI
        //        try
        //        {
        //            double averageRating = await _reviewService.GetAverageRatingAsync(id);
        //            // Si el averageRating es diferente al actual, actualizar y guardar
        //            if (Math.Abs(Convert.ToDouble(movie.AverageRating) - averageRating) > 0.001)
        //            {
        //                _logger.LogInformation("Updating movie {Id} average rating from {OldRating} to {NewRating}",
        //                    id, movie.AverageRating, averageRating);
        //                movie.AverageRating = Convert.ToDecimal(averageRating);
        //                await _db.SaveChangesAsync();
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            // Si falla obtener el rating, solo registrar el error pero continuar
        //            _logger.LogWarning(ex, "Failed to update average rating for movie {Id}", id);
        //        }

        //        response.Result = _mapper.Map<MovieDto>(movie);
        //        return Ok(response);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error getting movie with ID: {Id}", id);
        //        response.IsSuccess = false;
        //        response.Message = "Error retrieving movie";
        //        response.ErrorMessages = new List<string> { ex.Message };
        //        return StatusCode(500, response);
        //    }
        //}
        // En MoviesController.cs de MoviesAPI
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

                // Obtener el rating actualizado pero NO actualizar en base de datos
                double averageRating = await _reviewService.GetAverageRatingAsync(id);
                var movieDto = _mapper.Map<MovieDto>(movie);

                // Solo modificar el DTO para la respuesta, no actualizar la BD
                movieDto.AverageRating = Convert.ToDecimal(averageRating);

                response.Result = movieDto;
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
        [Authorize(Roles = "Admin,Manager")]
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
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult<ResponseDto>> UpdateMovie(int id, [FromBody] MovieUpdateDto movieUpdateDto)
        {
            var response = new ResponseDto();
            try
            {
                if (id <= 0 || movieUpdateDto == null)
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
        [Authorize(Roles = "Admin,Manager")]
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
        [Authorize(Roles = "Admin,Manager")]
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
        [Authorize(Roles = "Admin,Manager")]
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

        //[HttpPost("{id:int}/update-rating")]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        //[ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //public async Task<ActionResult<ResponseDto>> UpdateMovieAverageRating(int id)
        //{
        //    var response = new ResponseDto();
        //    try
        //    {
        //        _logger.LogInformation("Updating average rating for movie ID: {Id}", id);

        //        var movie = await _db.Movies.FindAsync(id);
        //        if (movie == null)
        //        {
        //            _logger.LogWarning("Movie not found with ID: {Id}", id);
        //            response.IsSuccess = false;
        //            response.Message = "Movie not found";
        //            return NotFound(response);
        //        }

        //        decimal oldRating = movie.AverageRating;

        //        try
        //        {
        //            // Obtener el nuevo rating promedio desde ReviewsAPI
        //            double averageRating = await _reviewService.GetAverageRatingAsync(id);

        //            // Actualizar con el nuevo promedio
        //            movie.AverageRating = Convert.ToDecimal(averageRating);

        //            _logger.LogInformation("Average rating updated for movie ID: {Id} from {OldRating} to {NewRating}",
        //                id, oldRating, movie.AverageRating);

        //            await _db.SaveChangesAsync();
        //        }
        //        catch (Exception ex)
        //        {
        //            // Si falla la comunicación, registrar el error pero continuar
        //            _logger.LogWarning(ex, "Failed to get average rating from ReviewsAPI for movie {Id}.", id);
        //            response.IsSuccess = false;
        //            response.Message = $"Error getting rating: {ex.Message}";
        //            return StatusCode(StatusCodes.Status500InternalServerError, response);
        //        }

        //        response.Result = movie.AverageRating;
        //        response.Message = "Average rating updated successfully";
        //        return Ok(response);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error in update-rating endpoint for movie {Id}", id);
        //        response.IsSuccess = false;
        //        response.Message = "Error updating average rating";
        //        response.ErrorMessages.Add(ex.Message);
        //        return StatusCode(StatusCodes.Status500InternalServerError, response);
        //    }
        //}

        // En MoviesController.cs, modificar el método UpdateMovieAverageRating

        [HttpPost("{id:int}/update-rating")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ResponseDto>> UpdateMovieAverageRating(int id)
        {
            var response = new ResponseDto();
            try
            {
                _logger.LogInformation("Updating average rating for movie ID: {Id}", id);

                var movie = await _db.Movies.FindAsync(id);
                if (movie == null)
                {
                    _logger.LogWarning("Movie not found with ID: {Id}", id);
                    response.IsSuccess = false;
                    response.Message = "Movie not found";
                    return NotFound(response);
                }

                decimal oldRating = movie.AverageRating;

                try
                {
                    // Invalidar la caché si estamos usando el servicio cacheado
                    if (_reviewService is CachedReviewAPIService cachedService)
                    {
                        cachedService.InvalidateMovieRatingCache(id);
                    }

                    // Obtener el nuevo rating promedio desde ReviewsAPI (ahora con caché)
                    double averageRating = await _reviewService.GetAverageRatingAsync(id);

                    // Actualizar con el nuevo promedio
                    movie.AverageRating = Convert.ToDecimal(averageRating);

                    _logger.LogInformation("Average rating updated for movie ID: {Id} from {OldRating} to {NewRating}",
                        id, oldRating, movie.AverageRating);

                    await _db.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    // Si falla la comunicación, registrar el error pero continuar
                    _logger.LogWarning(ex, "Failed to get average rating from ReviewsAPI for movie {Id}.", id);
                    response.IsSuccess = false;
                    response.Message = $"Error getting rating: {ex.Message}";
                    return StatusCode(StatusCodes.Status500InternalServerError, response);
                }

                response.Result = movie.AverageRating;
                response.Message = "Average rating updated successfully";
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in update-rating endpoint for movie {Id}", id);
                response.IsSuccess = false;
                response.Message = "Error updating average rating";
                response.ErrorMessages.Add(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }



        [HttpGet("{id:int}/exists")]
        public async Task<ActionResult<ResponseDto>> MovieExists(int id)
        {
            var response = new ResponseDto();
            try
            {
                var movieExists = await _db.Movies.AnyAsync(m => m.Id == id);

                if (movieExists)
                {
                    response.Result = true;
                    return Ok(response);
                }
                else
                {
                    // Esto es importante: no devuelvas 404 o el Circuit Breaker se activará
                    // En su lugar, devuelve 200 OK con un resultado false
                    response.Result = false;
                    return Ok(response);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if movie with ID: {Id} exists", id);
                response.IsSuccess = false;
                response.Message = "Error checking if movie exists";
                response.ErrorMessages = new List<string> { ex.Message };
                return StatusCode(500, response);
            }
        }

        // En MoviesController.cs del microservicio MoviesAPI
        [HttpGet("with-consistent-ratings")]
        public async Task<ActionResult<ResponseDto>> GetMoviesWithConsistentRatings()
        {
            var response = new ResponseDto();
            try
            {
                _logger.LogInformation("Getting all movies with consistent ratings");
                var movies = await _db.Movies
                    .Include(m => m.MovieGenres)
                    .ThenInclude(mg => mg.Genre)
                    .ToListAsync();

                // Transformar a DTOs
                var movieDtos = _mapper.Map<List<MovieDto>>(movies);

                // Para cada película, obtener el rating actualizado
                foreach (var movie in movieDtos)
                {
                    try
                    {
                        double rating = await _reviewService.GetAverageRatingAsync(movie.Id);
                        movie.AverageRating = (decimal)rating;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to get rating for movie {MovieId}, using stored value", movie.Id);
                        // Mantenemos el rating actual en caso de error
                    }
                }

                response.Result = movieDtos;
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting movies with consistent ratings");
                response.IsSuccess = false;
                response.Message = "Error retrieving movies";
                response.ErrorMessages = new List<string> { ex.Message };
                return StatusCode(500, response);
            }
        }


    }
}
