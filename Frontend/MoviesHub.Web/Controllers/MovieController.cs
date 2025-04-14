using Microsoft.AspNetCore.Mvc;
using MoviesHub.Web.Models;
using MoviesHub.Web.Service.IService;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MoviesHub.Web.Controllers
{
    public class MovieController : Controller
    {
        private readonly IMovieService _movieService;
        private readonly IGenreService _genreService;

        public MovieController(IMovieService movieService, IGenreService genreService)
        {
            _movieService = movieService;
            _genreService = genreService;
        }

        public async Task<IActionResult> Index()
        {
            var response = await _movieService.GetAllMoviesAsync();
            List<MovieDto>? movies = new();

            if (response != null && response.IsSuccess)
            {
                //movies = JsonConvert.DeserializeObject<List<MovieDto>>(Convert.ToString(response.Result));
                var json = Convert.ToString(response.Result);
                var extractedResult = JObject.Parse(json)["result"].ToString();
                movies = JsonConvert.DeserializeObject<List<MovieDto>>(extractedResult);
            }
            else
            {
                TempData["error"] = response?.Message ?? "Error retrieving movies";
            }

            return View(movies);
        }

        public async Task<IActionResult> Create()
        {
            // Get all genres for selection
            var genreResponse = await _genreService.GetAllGenresAsync();
            if (genreResponse != null && genreResponse.IsSuccess)
            {
                ViewBag.Genres = JsonConvert.DeserializeObject<List<GenreDto>>(Convert.ToString(genreResponse.Result));
            }
            else
            {
                ViewBag.Genres = new List<GenreDto>();
                TempData["error"] = genreResponse?.Message ?? "Error retrieving genres";
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MovieCreateDto model)
        {
            if (ModelState.IsValid)
            {
                var response = await _movieService.CreateMovieAsync(model);
                if (response != null && response.IsSuccess)
                {
                    TempData["success"] = "Movie created successfully";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["error"] = response?.Message ?? "Error creating movie";
                }
            }

            // Repopulate genres for selection if validation fails
            var genreResponse = await _genreService.GetAllGenresAsync();
            if (genreResponse != null && genreResponse.IsSuccess)
            {
                ViewBag.Genres = JsonConvert.DeserializeObject<List<GenreDto>>(Convert.ToString(genreResponse.Result));
            }
            else
            {
                ViewBag.Genres = new List<GenreDto>();
            }

            return View(model);
        }

        public async Task<IActionResult> Edit(int id)
        {
            // Get movie data
            var response = await _movieService.GetMovieByIdAsync(id);
            if (response != null && response.IsSuccess)
            {
                MovieDto movie = JsonConvert.DeserializeObject<MovieDto>(Convert.ToString(response.Result));

                // Convert to update DTO
                MovieUpdateDto updateDto = new()
                {
                    Title = movie.Title,
                    Description = movie.Description,
                    ReleaseYear = movie.ReleaseYear,
                    ImageUrl = movie.ImageUrl,
                    GenreIds = movie.Genres.Select(g => g.Id).ToList()
                };

                // Get all genres for selection
                var genreResponse = await _genreService.GetAllGenresAsync();
                if (genreResponse != null && genreResponse.IsSuccess)
                {
                    ViewBag.Genres = JsonConvert.DeserializeObject<List<GenreDto>>(Convert.ToString(genreResponse.Result));
                }
                else
                {
                    ViewBag.Genres = new List<GenreDto>();
                    TempData["error"] = genreResponse?.Message ?? "Error retrieving genres";
                }

                return View(updateDto);
            }

            TempData["error"] = response?.Message ?? "Error retrieving movie";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, MovieUpdateDto model)
        {
            if (ModelState.IsValid)
            {
                var response = await _movieService.UpdateMovieAsync(id, model);
                if (response != null && response.IsSuccess)
                {
                    TempData["success"] = "Movie updated successfully";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["error"] = response?.Message ?? "Error updating movie";
                }
            }

            // Repopulate genres for selection if validation fails
            var genreResponse = await _genreService.GetAllGenresAsync();
            if (genreResponse != null && genreResponse.IsSuccess)
            {
                ViewBag.Genres = JsonConvert.DeserializeObject<List<GenreDto>>(Convert.ToString(genreResponse.Result));
            }
            else
            {
                ViewBag.Genres = new List<GenreDto>();
            }

            return View(model);
        }

        public async Task<IActionResult> Details(int id)
        {
            var response = await _movieService.GetMovieByIdAsync(id);
            if (response != null && response.IsSuccess)
            {
                MovieDto movie = JsonConvert.DeserializeObject<MovieDto>(Convert.ToString(response.Result));
                return View(movie);
            }

            TempData["error"] = response?.Message ?? "Error retrieving movie details";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var response = await _movieService.GetMovieByIdAsync(id);
            if (response != null && response.IsSuccess)
            {
                MovieDto movie = JsonConvert.DeserializeObject<MovieDto>(Convert.ToString(response.Result));
                return View(movie);
            }

            TempData["error"] = response?.Message ?? "Error retrieving movie";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var response = await _movieService.DeleteMovieAsync(id);
            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Movie deleted successfully";
            }
            else
            {
                TempData["error"] = response?.Message ?? "Error deleting movie";
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Restore(int id)
        {
            var response = await _movieService.RestoreMovieAsync(id);
            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Movie restored successfully";
            }
            else
            {
                TempData["error"] = response?.Message ?? "Error restoring movie";
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> ByGenre(int genreId)
        {
            var response = await _movieService.GetMoviesByGenreAsync(genreId);
            List<MovieDto>? movies = new();

            if (response != null && response.IsSuccess)
            {
                movies = JsonConvert.DeserializeObject<List<MovieDto>>(Convert.ToString(response.Result));
            }
            else
            {
                TempData["error"] = response?.Message ?? "Error retrieving movies by genre";
            }

            // Get genre name for display
            var genreResponse = await _genreService.GetGenreByIdAsync(genreId);
            if (genreResponse != null && genreResponse.IsSuccess)
            {
                GenreDto genre = JsonConvert.DeserializeObject<GenreDto>(Convert.ToString(genreResponse.Result));
                ViewBag.GenreName = genre.Name;
            }

            ViewBag.GenreId = genreId;
            return View(movies);
        }
    }
}
