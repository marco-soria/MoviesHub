using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MoviesHub.Services.MoviesAPI.Data;
using MoviesHub.Services.MoviesAPI.Models.Dto;
using MoviesHub.Services.MoviesAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace MoviesHub.Services.MoviesAPI.Controllers
{
    [Route("api/genres")]
    [ApiController]
    public class GenresController : ControllerBase
    {
        private readonly MovieDbContext _db;
        private readonly IMapper _mapper;
        private readonly ILogger<GenresController> _logger;

        public GenresController(MovieDbContext db, IMapper mapper, ILogger<GenresController> logger)
        {
            _db = db;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<ResponseDto>> GetGenres()
        {
            var response = new ResponseDto();
            try
            {
                _logger.LogInformation("Getting all genres");
                var genres = await _db.Genres.ToListAsync();
                response.Result = _mapper.Map<List<GenreDto>>(genres);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all genres");
                response.IsSuccess = false;
                response.Message = "Error retrieving genres";
                response.ErrorMessages.Add(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ResponseDto>> GetGenre(int id)
        {
            var response = new ResponseDto();
            try
            {
                var genre = await _db.Genres.FindAsync(id);
                if (genre == null)
                {
                    _logger.LogWarning("Genre with ID: {Id} not found", id);
                    response.IsSuccess = false;
                    response.Message = "Genre not found";
                    return NotFound(response);
                }

                response.Result = _mapper.Map<GenreDto>(genre);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting genre with ID: {Id}", id);
                response.IsSuccess = false;
                response.Message = "Error retrieving genre";
                response.ErrorMessages.Add(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }

        [HttpPost]
        public async Task<ActionResult<ResponseDto>> CreateGenre([FromBody] GenreCreateDto genreCreateDto)
        {
            var response = new ResponseDto();
            try
            {
                if (genreCreateDto == null)
                {
                    response.IsSuccess = false;
                    response.Message = "Invalid data provided";
                    return BadRequest(response);
                }

                // Check for duplicate genre name
                if (await _db.Genres.AnyAsync(g => g.Name.ToLower() == genreCreateDto.Name.ToLower()))
                {
                    response.IsSuccess = false;
                    response.Message = "A genre with this name already exists";
                    return Conflict(response);
                }

                var genre = _mapper.Map<Genre>(genreCreateDto);
                await _db.Genres.AddAsync(genre);
                await _db.SaveChangesAsync();

                response.Result = _mapper.Map<GenreDto>(genre);
                response.Message = "Genre created successfully";

                return CreatedAtAction(nameof(GetGenre), new { id = genre.Id }, response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating genre");
                response.IsSuccess = false;
                response.Message = "Error creating genre";
                response.ErrorMessages.Add(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<ResponseDto>> UpdateGenre(int id, [FromBody] GenreUpdateDto genreUpdateDto)
        {
            var response = new ResponseDto();
            try
            {
                if (genreUpdateDto == null)
                {
                    response.IsSuccess = false;
                    response.Message = "Invalid data provided";
                    return BadRequest(response);
                }

                var genre = await _db.Genres.FindAsync(id);
                if (genre == null)
                {
                    _logger.LogWarning("Genre with ID: {Id} not found for update", id);
                    response.IsSuccess = false;
                    response.Message = "Genre not found";
                    return NotFound(response);
                }

                // Check for duplicate genre name
                if (await _db.Genres.AnyAsync(g => g.Name.ToLower() == genreUpdateDto.Name.ToLower() && g.Id != id))
                {
                    response.IsSuccess = false;
                    response.Message = "A genre with this name already exists";
                    return Conflict(response);
                }

                _mapper.Map(genreUpdateDto, genre);
                await _db.SaveChangesAsync();

                response.Message = "Genre updated successfully";
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating genre with ID: {Id}", id);
                response.IsSuccess = false;
                response.Message = "Error updating genre";
                response.ErrorMessages.Add(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<ResponseDto>> DeleteGenre(int id)
        {
            var response = new ResponseDto();
            try
            {
                var genre = await _db.Genres.FindAsync(id);
                if (genre == null)
                {
                    _logger.LogWarning("Genre with ID: {Id} not found for deletion", id);
                    response.IsSuccess = false;
                    response.Message = "Genre not found";
                    return NotFound(response);
                }

                // Soft delete the genre
                genre.IsDeleted = true;
                genre.DeletedAt = DateTime.UtcNow;
                await _db.SaveChangesAsync();

                response.Message = "Genre deleted successfully";
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting genre with ID: {Id}", id);
                response.IsSuccess = false;
                response.Message = "Error deleting genre";
                response.ErrorMessages.Add(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }

        // Additional endpoint to restore a soft-deleted genre
        [HttpPatch("{id:int}/restore")]
        public async Task<ActionResult<ResponseDto>> RestoreGenre(int id)
        {
            var response = new ResponseDto();
            try
            {
                var genre = await _db.Genres
                    .IgnoreQueryFilters() // Important to find soft-deleted genres
                    .FirstOrDefaultAsync(g => g.Id == id && g.IsDeleted);

                if (genre == null)
                {
                    response.IsSuccess = false;
                    response.Message = "Deleted genre not found";
                    return NotFound(response);
                }

                genre.IsDeleted = false;
                genre.DeletedAt = null;

                await _db.SaveChangesAsync();

                response.Message = "Genre restored successfully";
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error restoring genre with ID: {Id}", id);
                response.IsSuccess = false;
                response.Message = "Error restoring genre";
                response.ErrorMessages.Add(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }

        // Get all movies for a specific genre
        [HttpGet("{id:int}/movies")]
        public async Task<ActionResult<ResponseDto>> GetMoviesForGenre(int id)
        {
            var response = new ResponseDto();
            try
            {
                if (!await _db.Genres.AnyAsync(g => g.Id == id))
                {
                    response.IsSuccess = false;
                    response.Message = "Genre not found";
                    return NotFound(response);
                }

                var movies = await _db.Movies
                    .Include(m => m.MovieGenres)
                    .ThenInclude(mg => mg.Genre)
                    .Where(m => m.MovieGenres.Any(mg => mg.GenreId == id))
                    .ToListAsync();

                response.Result = _mapper.Map<List<MovieDto>>(movies);
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting movies for genre with ID: {Id}", id);
                response.IsSuccess = false;
                response.Message = "Error retrieving movies for genre";
                response.ErrorMessages.Add(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }


        }

        [HttpGet("deleted")]
        public async Task<ActionResult<ResponseDto>> GetDeletedGenres()
        {
            var response = new ResponseDto();
            try
            {
                var deletedGenres = await _db.Genres
                    .IgnoreQueryFilters() // Ignore the global query filter for IsDeleted
                    .Where(m => m.IsDeleted)
                    .Include(m => m.MovieGenres)
                    .ThenInclude(mv => mv.Movie)
                    .ToListAsync();

                response.Result = _mapper.Map<List<GenreDto>>(deletedGenres);
                response.IsSuccess = true;
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving deleted genres");
                response.IsSuccess = false;
                response.Message = "Error retrieving deleted genres";
                response.ErrorMessages = new List<string> { ex.Message };
                return StatusCode(500, response);
            }
        }
    }
}
//namespace MoviesHub.Services.MoviesAPI.Controllers
//{
//    [Route("api/genres")]
//    [ApiController]
//    public class GenresController : ControllerBase
//    {
//        private readonly MovieDbContext _db;
//        private readonly IMapper _mapper;
//        private readonly ILogger<GenresController> _logger;

//        public GenresController(MovieDbContext db, IMapper mapper, ILogger<GenresController> logger)
//        {
//            _db = db;
//            _mapper = mapper;
//            _logger = logger;
//        }

//        [HttpGet]
//        public async Task<ActionResult<IEnumerable<GenreDto>>> GetGenres()
//        {
//            _logger.LogInformation("Getting all genres");
//            var genres = await _db.Genres.ToListAsync();
//            return Ok(_mapper.Map<List<GenreDto>>(genres));
//        }

//        [HttpGet("{id:int}")]
//        public async Task<ActionResult<GenreDto>> GetGenre(int id)
//        {
//            var genre = await _db.Genres.FindAsync(id);
//            if (genre == null)
//            {
//                _logger.LogWarning("Genre with ID: {Id} not found", id);
//                return NotFound();
//            }

//            return Ok(_mapper.Map<GenreDto>(genre));
//        }

//        [HttpPost]
//        public async Task<ActionResult<GenreDto>> CreateGenre([FromBody] GenreCreateDto genreCreateDto)
//        {
//            if (genreCreateDto == null)
//                return BadRequest();

//            // Check for duplicate genre name
//            if (await _db.Genres.AnyAsync(g => g.Name.ToLower() == genreCreateDto.Name.ToLower()))
//            {
//                return Conflict(new { Message = "A genre with this name already exists." });
//            }

//            var genre = _mapper.Map<Genre>(genreCreateDto);
//            await _db.Genres.AddAsync(genre);
//            await _db.SaveChangesAsync();

//            return CreatedAtAction(nameof(GetGenre), new { id = genre.Id }, _mapper.Map<GenreDto>(genre));
//        }

//        [HttpPut("{id:int}")]
//        public async Task<IActionResult> UpdateGenre(int id, [FromBody] GenreUpdateDto genreUpdateDto)
//        {
//            if (genreUpdateDto == null)
//                return BadRequest();

//            var genre = await _db.Genres.FindAsync(id);
//            if (genre == null)
//            {
//                _logger.LogWarning("Genre with ID: {Id} not found for update", id);
//                return NotFound();
//            }

//            // Check for duplicate genre name
//            if (await _db.Genres.AnyAsync(g => g.Name.ToLower() == genreUpdateDto.Name.ToLower() && g.Id != id))
//            {
//                return Conflict(new { Message = "A genre with this name already exists." });
//            }

//            _mapper.Map(genreUpdateDto, genre);
//            await _db.SaveChangesAsync();

//            return NoContent();
//        }

//        [HttpDelete("{id:int}")]
//        public async Task<IActionResult> DeleteGenre(int id)
//        {
//            var genre = await _db.Genres.FindAsync(id);
//            if (genre == null)
//            {
//                _logger.LogWarning("Genre with ID: {Id} not found for deletion", id);
//                return NotFound();
//            }

//            // Check if genre is used by any movies
//            //var isUsed = await _db.MovieGenres.AnyAsync(mg => mg.GenreId == id);
//            //if (isUsed)
//            //{
//            //    // Soft delete since it's being used
//            //    genre.IsDeleted = true;
//            //    genre.DeletedAt = DateTime.UtcNow;
//            //    await _db.SaveChangesAsync();
//            //    return NoContent();
//            //}
//            genre.IsDeleted = true;
//            genre.DeletedAt = DateTime.UtcNow;
//            await _db.SaveChangesAsync();
//            return NoContent();

//            // Hard delete if not used
//            //_db.Genres.Remove(genre);
//            //await _db.SaveChangesAsync();
//            //return NoContent();
//        }

//        // Additional endpoint to restore a soft-deleted genre
//        [HttpPatch("{id:int}/restore")]
//        public async Task<IActionResult> RestoreGenre(int id)
//        {
//            var genre = await _db.Genres
//                .IgnoreQueryFilters() // Important to find soft-deleted genres
//                .FirstOrDefaultAsync(g => g.Id == id && g.IsDeleted);

//            if (genre == null)
//            {
//                return NotFound();
//            }

//            genre.IsDeleted = false;
//            genre.DeletedAt = null;

//            await _db.SaveChangesAsync();
//            return NoContent();
//        }

//        // Get all movies for a specific genre
//        [HttpGet("{id:int}/movies")]
//        public async Task<ActionResult<IEnumerable<MovieDto>>> GetMoviesForGenre(int id)
//        {
//            if (!await _db.Genres.AnyAsync(g => g.Id == id))
//            {
//                return NotFound();
//            }

//            var movies = await _db.Movies
//                .Include(m => m.MovieGenres)
//                .ThenInclude(mg => mg.Genre)
//                .Where(m => m.MovieGenres.Any(mg => mg.GenreId == id))
//                .ToListAsync();

//            return Ok(_mapper.Map<List<MovieDto>>(movies));
//        }
//    }
//}
