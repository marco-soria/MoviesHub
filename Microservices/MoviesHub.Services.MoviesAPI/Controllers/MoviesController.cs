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
        public async Task<ActionResult<IEnumerable<MovieDto>>> GetMovies()
        {
            _logger.LogInformation("Getting all movies");
            var movies = await _db.Movies
                .Include(m => m.MovieGenres)
                .ThenInclude(mg => mg.Genre)
                .ToListAsync();

            return Ok(_mapper.Map<List<MovieDto>>(movies));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<MovieDto>> GetMovie(int id)
        {
            var movie = await _db.Movies
                .Include(m => m.MovieGenres)
                .ThenInclude(mg => mg.Genre)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (movie == null)
            {
                _logger.LogWarning("Movie with ID: {Id} not found", id);
                return NotFound();
            }

            return Ok(_mapper.Map<MovieDto>(movie));
        }

        [HttpPost]
        public async Task<ActionResult<MovieDto>> CreateMovie([FromBody] MovieCreateDto movieCreateDto)
        {
            if (movieCreateDto == null)
                return BadRequest();

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

            return CreatedAtAction(nameof(GetMovie), new { id = movie.Id }, _mapper.Map<MovieDto>(createdMovie));
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateMovie(int id, [FromBody] MovieUpdateDto movieUpdateDto)
        {
            if (movieUpdateDto == null)
                return BadRequest();

            var existingMovie = await _db.Movies
                .Include(m => m.MovieGenres)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (existingMovie == null)
            {
                _logger.LogWarning("Movie with ID: {Id} not found for update", id);
                return NotFound();
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
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteMovie(int id)
        {
            var movie = await _db.Movies.FindAsync(id);
            if (movie == null)
            {
                _logger.LogWarning("Movie with ID: {Id} not found for deletion", id);
                return NotFound();
            }

            // Soft delete
            movie.IsDeleted = true;
            movie.DeletedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            return NoContent();
        }

        // Additional endpoint to restore a soft-deleted movie
        [HttpPatch("{id:int}/restore")]
        public async Task<IActionResult> RestoreMovie(int id)
        {
            var movie = await _db.Movies
                .IgnoreQueryFilters() // Important to find soft-deleted movies
                .FirstOrDefaultAsync(m => m.Id == id && m.IsDeleted);

            if (movie == null)
            {
                return NotFound();
            }

            movie.IsDeleted = false;
            movie.DeletedAt = null;

            await _db.SaveChangesAsync();
            return NoContent();
        }

        // Get movies by genre
        [HttpGet("bygenre/{genreId:int}")]
        public async Task<ActionResult<IEnumerable<MovieDto>>> GetMoviesByGenre(int genreId)
        {
            var movies = await _db.Movies
                .Include(m => m.MovieGenres)
                .ThenInclude(mg => mg.Genre)
                .Where(m => m.MovieGenres.Any(mg => mg.GenreId == genreId))
                .ToListAsync();

            return Ok(_mapper.Map<List<MovieDto>>(movies));
        }
    }
}
