using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MoviesHub.Services.MoviesAPI.Data;
using MoviesHub.Services.MoviesAPI.Models.Dto;
using MoviesHub.Services.MoviesAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace MoviesHub.Services.MoviesAPI.Controllers
{
    [Route("api/movie-genres")]
    [ApiController]
    public class MovieGenresController : ControllerBase
    {
        private readonly MovieDbContext _db;
        private readonly IMapper _mapper;
        private readonly ILogger<MovieGenresController> _logger;

        public MovieGenresController(MovieDbContext db, IMapper mapper, ILogger<MovieGenresController> logger)
        {
            _db = db;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<ResponseDto>> GetAllMovieGenres()
        {
            var response = new ResponseDto();
            try
            {
                _logger.LogInformation("Getting all movie-genre relationships");

                var movieGenres = await _db.MovieGenres
                    .Include(mg => mg.Movie)
                    .Include(mg => mg.Genre)
                    .Select(mg => new MovieGenreDto
                    {
                        MovieId = mg.MovieId,
                        GenreId = mg.GenreId,
                        MovieTitle = mg.Movie.Title,
                        GenreName = mg.Genre.Name
                    })
                    .ToListAsync();

                response.Result = movieGenres;
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all movie-genre relationships");
                response.IsSuccess = false;
                response.Message = "Error retrieving movie-genre relationships";
                response.ErrorMessages.Add(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }

        [HttpGet("{movieId:int}/{genreId:int}")]
        public async Task<ActionResult<ResponseDto>> GetMovieGenre(int movieId, int genreId)
        {
            var response = new ResponseDto();
            try
            {
                var movieGenre = await _db.MovieGenres
                    .Include(mg => mg.Movie)
                    .Include(mg => mg.Genre)
                    .FirstOrDefaultAsync(mg => mg.MovieId == movieId && mg.GenreId == genreId);

                if (movieGenre == null)
                {
                    _logger.LogWarning("Relationship between Movie ID: {MovieId} and Genre ID: {GenreId} not found", movieId, genreId);
                    response.IsSuccess = false;
                    response.Message = "Movie-genre relationship not found";
                    return NotFound(response);
                }

                var dto = new MovieGenreDto
                {
                    MovieId = movieGenre.MovieId,
                    GenreId = movieGenre.GenreId,
                    MovieTitle = movieGenre.Movie.Title,
                    GenreName = movieGenre.Genre.Name
                };

                response.Result = dto;
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting movie-genre relationship");
                response.IsSuccess = false;
                response.Message = "Error retrieving movie-genre relationship";
                response.ErrorMessages.Add(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult<ResponseDto>> CreateMovieGenre([FromBody] MovieGenreCreateDto dto)
        {
            var response = new ResponseDto();
            try
            {
                // Validate movie exists
                var movie = await _db.Movies.FindAsync(dto.MovieId);
                if (movie == null)
                {
                    response.IsSuccess = false;
                    response.Message = "Movie not found";
                    return BadRequest(response);
                }

                // Validate genre exists
                var genre = await _db.Genres.FindAsync(dto.GenreId);
                if (genre == null)
                {
                    response.IsSuccess = false;
                    response.Message = "Genre not found";
                    return BadRequest(response);
                }

                // Check if relationship already exists
                var exists = await _db.MovieGenres.AnyAsync(mg =>
                    mg.MovieId == dto.MovieId &&
                    mg.GenreId == dto.GenreId);

                if (exists)
                {
                    response.IsSuccess = false;
                    response.Message = "This movie-genre relationship already exists";
                    return Conflict(response);
                }

                // Create the relationship
                var movieGenre = new MovieGenre
                {
                    MovieId = dto.MovieId,
                    GenreId = dto.GenreId
                };

                await _db.MovieGenres.AddAsync(movieGenre);
                await _db.SaveChangesAsync();

                // Return the created relationship with names
                var result = new MovieGenreDto
                {
                    MovieId = movieGenre.MovieId,
                    GenreId = movieGenre.GenreId,
                    MovieTitle = movie.Title,
                    GenreName = genre.Name
                };

                response.Result = result;
                response.Message = "Movie-genre relationship created successfully";

                return CreatedAtAction(
                    nameof(GetMovieGenre),
                    new { movieId = dto.MovieId, genreId = dto.GenreId },
                    response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating movie-genre relationship");
                response.IsSuccess = false;
                response.Message = "Error creating movie-genre relationship";
                response.ErrorMessages.Add(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }

        [HttpDelete("{movieId:int}/{genreId:int}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<ActionResult<ResponseDto>> DeleteMovieGenre(int movieId, int genreId)
        {
            var response = new ResponseDto();
            try
            {
                var movieGenre = await _db.MovieGenres
                    .FirstOrDefaultAsync(mg => mg.MovieId == movieId && mg.GenreId == genreId);

                if (movieGenre == null)
                {
                    _logger.LogWarning("Relationship between Movie ID: {MovieId} and Genre ID: {GenreId} not found for deletion",
                        movieId, genreId);
                    response.IsSuccess = false;
                    response.Message = "Movie-genre relationship not found";
                    return NotFound(response);
                }

                _db.MovieGenres.Remove(movieGenre);
                await _db.SaveChangesAsync();

                response.Message = "Movie-genre relationship deleted successfully";
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting movie-genre relationship");
                response.IsSuccess = false;
                response.Message = "Error deleting movie-genre relationship";
                response.ErrorMessages.Add(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }

        // Get all genres for a movie
        [HttpGet("movie/{movieId:int}/genres")]
        public async Task<ActionResult<ResponseDto>> GetGenresForMovie(int movieId)
        {
            var response = new ResponseDto();
            try
            {
                if (!await _db.Movies.AnyAsync(m => m.Id == movieId))
                {
                    response.IsSuccess = false;
                    response.Message = "Movie not found";
                    return NotFound(response);
                }

                var genres = await _db.MovieGenres
                    .Where(mg => mg.MovieId == movieId)
                    .Include(mg => mg.Genre)
                    .Select(mg => _mapper.Map<GenreDto>(mg.Genre))
                    .ToListAsync();

                response.Result = genres;
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting genres for movie");
                response.IsSuccess = false;
                response.Message = "Error retrieving genres for movie";
                response.ErrorMessages.Add(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }

        // Get all movies for a genre
        [HttpGet("genre/{genreId:int}/movies")]
        public async Task<ActionResult<ResponseDto>> GetMoviesForGenre(int genreId)
        {
            var response = new ResponseDto();
            try
            {
                if (!await _db.Genres.AnyAsync(g => g.Id == genreId))
                {
                    response.IsSuccess = false;
                    response.Message = "Genre not found";
                    return NotFound(response);
                }

                var movies = await _db.MovieGenres
                    .Where(mg => mg.GenreId == genreId)
                    .Include(mg => mg.Movie)
                    .ThenInclude(m => m.MovieGenres)
                    .ThenInclude(mg => mg.Genre)
                    .Select(mg => _mapper.Map<MovieDto>(mg.Movie))
                    .ToListAsync();

                response.Result = movies;
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting movies for genre");
                response.IsSuccess = false;
                response.Message = "Error retrieving movies for genre";
                response.ErrorMessages.Add(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }
    }
}
//namespace MoviesHub.Services.MoviesAPI.Controllers
//{
//    [Route("api/movie-genres")]
//    [ApiController]
//    public class MovieGenresController : ControllerBase
//    {
//        private readonly MovieDbContext _db;
//        private readonly IMapper _mapper;
//        private readonly ILogger<MovieGenresController> _logger;

//        public MovieGenresController(MovieDbContext db, IMapper mapper, ILogger<MovieGenresController> logger)
//        {
//            _db = db;
//            _mapper = mapper;
//            _logger = logger;
//        }

//        [HttpGet]
//        public async Task<ActionResult<IEnumerable<MovieGenreDto>>> GetAllMovieGenres()
//        {
//            _logger.LogInformation("Getting all movie-genre relationships");

//            var movieGenres = await _db.MovieGenres
//                .Include(mg => mg.Movie)
//                .Include(mg => mg.Genre)
//                .Select(mg => new MovieGenreDto
//                {
//                    MovieId = mg.MovieId,
//                    GenreId = mg.GenreId,
//                    MovieTitle = mg.Movie.Title,
//                    GenreName = mg.Genre.Name
//                })
//                .ToListAsync();

//            return Ok(movieGenres);
//        }

//        [HttpGet("{movieId:int}/{genreId:int}")]
//        public async Task<ActionResult<MovieGenreDto>> GetMovieGenre(int movieId, int genreId)
//        {
//            var movieGenre = await _db.MovieGenres
//                .Include(mg => mg.Movie)
//                .Include(mg => mg.Genre)
//                .FirstOrDefaultAsync(mg => mg.MovieId == movieId && mg.GenreId == genreId);

//            if (movieGenre == null)
//            {
//                _logger.LogWarning("Relationship between Movie ID: {MovieId} and Genre ID: {GenreId} not found", movieId, genreId);
//                return NotFound();
//            }

//            var dto = new MovieGenreDto
//            {
//                MovieId = movieGenre.MovieId,
//                GenreId = movieGenre.GenreId,
//                MovieTitle = movieGenre.Movie.Title,
//                GenreName = movieGenre.Genre.Name
//            };

//            return Ok(dto);
//        }

//        [HttpPost]
//        public async Task<ActionResult<MovieGenreDto>> CreateMovieGenre([FromBody] MovieGenreCreateDto dto)
//        {
//            // Validate movie exists
//            var movie = await _db.Movies.FindAsync(dto.MovieId);
//            if (movie == null)
//                return BadRequest(new { Message = "Movie not found" });

//            // Validate genre exists
//            var genre = await _db.Genres.FindAsync(dto.GenreId);
//            if (genre == null)
//                return BadRequest(new { Message = "Genre not found" });

//            // Check if relationship already exists
//            var exists = await _db.MovieGenres.AnyAsync(mg =>
//                mg.MovieId == dto.MovieId &&
//                mg.GenreId == dto.GenreId);

//            if (exists)
//                return Conflict(new { Message = "This movie-genre relationship already exists" });

//            // Create the relationship
//            var movieGenre = new MovieGenre
//            {
//                MovieId = dto.MovieId,
//                GenreId = dto.GenreId
//            };

//            await _db.MovieGenres.AddAsync(movieGenre);
//            await _db.SaveChangesAsync();

//            // Return the created relationship with names
//            var result = new MovieGenreDto
//            {
//                MovieId = movieGenre.MovieId,
//                GenreId = movieGenre.GenreId,
//                MovieTitle = movie.Title,
//                GenreName = genre.Name
//            };

//            return CreatedAtAction(
//                nameof(GetMovieGenre),
//                new { movieId = dto.MovieId, genreId = dto.GenreId },
//                result);
//        }

//        [HttpDelete("{movieId:int}/{genreId:int}")]
//        public async Task<IActionResult> DeleteMovieGenre(int movieId, int genreId)
//        {
//            var movieGenre = await _db.MovieGenres
//                .FirstOrDefaultAsync(mg => mg.MovieId == movieId && mg.GenreId == genreId);

//            if (movieGenre == null)
//            {
//                _logger.LogWarning("Relationship between Movie ID: {MovieId} and Genre ID: {GenreId} not found for deletion",
//                    movieId, genreId);
//                return NotFound();
//            }

//            _db.MovieGenres.Remove(movieGenre);
//            await _db.SaveChangesAsync();

//            return NoContent();
//        }

//        // Get all genres for a movie
//        [HttpGet("movie/{movieId:int}/genres")]
//        public async Task<ActionResult<IEnumerable<GenreDto>>> GetGenresForMovie(int movieId)
//        {
//            if (!await _db.Movies.AnyAsync(m => m.Id == movieId))
//                return NotFound(new { Message = "Movie not found" });

//            var genres = await _db.MovieGenres
//                .Where(mg => mg.MovieId == movieId)
//                .Include(mg => mg.Genre)
//                .Select(mg => _mapper.Map<GenreDto>(mg.Genre))
//                .ToListAsync();

//            return Ok(genres);
//        }

//        // Get all movies for a genre
//        [HttpGet("genre/{genreId:int}/movies")]
//        public async Task<ActionResult<IEnumerable<MovieDto>>> GetMoviesForGenre(int genreId)
//        {
//            if (!await _db.Genres.AnyAsync(g => g.Id == genreId))
//                return NotFound(new { Message = "Genre not found" });

//            var movies = await _db.MovieGenres
//                .Where(mg => mg.GenreId == genreId)
//                .Include(mg => mg.Movie)
//                .ThenInclude(m => m.MovieGenres)
//                .ThenInclude(mg => mg.Genre)
//                .Select(mg => _mapper.Map<MovieDto>(mg.Movie))
//                .ToListAsync();

//            return Ok(movies);
//        }
//    }
//}

