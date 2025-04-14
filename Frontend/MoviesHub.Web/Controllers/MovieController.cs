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
            var genreResponse = await _genreService.GetAllGenresAsync();
            if (genreResponse != null && genreResponse.IsSuccess)
            {
                var json = Convert.ToString(genreResponse.Result);
                var extractedResult = JObject.Parse(json)["result"].ToString();
                ViewBag.Genres = JsonConvert.DeserializeObject<List<GenreDto>>(extractedResult);
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

            var genreResponse = await _genreService.GetAllGenresAsync();
            if (genreResponse != null && genreResponse.IsSuccess)
            {
                var json = Convert.ToString(genreResponse.Result);
                var extractedResult = JObject.Parse(json)["result"].ToString();
                ViewBag.Genres = JsonConvert.DeserializeObject<List<GenreDto>>(extractedResult);
            }
            else
            {
                ViewBag.Genres = new List<GenreDto>();
            }

            return View(model);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var response = await _movieService.GetMovieByIdAsync(id);
            if (response != null && response.IsSuccess)
            {
                var json = Convert.ToString(response.Result);
                var extractedResult = JObject.Parse(json)["result"].ToString();
                MovieDto movie = JsonConvert.DeserializeObject<MovieDto>(extractedResult);

                MovieUpdateDto updateDto = new()
                {
                    Title = movie.Title,
                    Description = movie.Description,
                    ReleaseYear = movie.ReleaseYear,
                    ImageUrl = movie.ImageUrl,
                    GenreIds = movie.Genres.Select(g => g.Id).ToList()
                };

                var genreResponse = await _genreService.GetAllGenresAsync();
                if (genreResponse != null && genreResponse.IsSuccess)
                {
                    var genreJson = Convert.ToString(genreResponse.Result);
                    var genreExtractedResult = JObject.Parse(genreJson)["result"].ToString();
                    ViewBag.Genres = JsonConvert.DeserializeObject<List<GenreDto>>(genreExtractedResult);
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

            var genreResponse = await _genreService.GetAllGenresAsync();
            if (genreResponse != null && genreResponse.IsSuccess)
            {
                var json = Convert.ToString(genreResponse.Result);
                var extractedResult = JObject.Parse(json)["result"].ToString();
                ViewBag.Genres = JsonConvert.DeserializeObject<List<GenreDto>>(extractedResult);
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
                var json = Convert.ToString(response.Result);
                var extractedResult = JObject.Parse(json)["result"].ToString();
                MovieDto movie = JsonConvert.DeserializeObject<MovieDto>(extractedResult);
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
                var json = Convert.ToString(response.Result);
                var extractedResult = JObject.Parse(json)["result"].ToString();
                MovieDto movie = JsonConvert.DeserializeObject<MovieDto>(extractedResult);
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

        [HttpPost]
        [ValidateAntiForgeryToken]
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

            return RedirectToAction(nameof(DeletedMovies));
        }

        public async Task<IActionResult> DeletedMovies()
        {
            var response = await _movieService.GetDeletedMoviesAsync();
            List<MovieDto>? movies = new();

            if (response != null && response.IsSuccess)
            {
                var json = Convert.ToString(response.Result);
                var jsonObject = JsonConvert.DeserializeObject<dynamic>(json);
                movies = JsonConvert.DeserializeObject<List<MovieDto>>(Convert.ToString(jsonObject.result));
            }
            else
            {
                TempData["error"] = response?.Message ?? "Error retrieving deleted movies";
            }

            return View("DeletedMovies", movies);
        }

        public async Task<IActionResult> ByGenre(int genreId = 0)
        {
            List<MovieDto>? movies = new();
            ResponseDto response;

            if (genreId == 0)
            {
                // Get all movies when "All" is selected
                response = await _movieService.GetAllMoviesAsync();
            }
            else
            {
                // Get movies filtered by genre
                response = await _movieService.GetMoviesByGenreAsync(genreId);
            }

            if (response != null && response.IsSuccess)
            {
                var json = Convert.ToString(response.Result);
                var extractedResult = JObject.Parse(json)["result"].ToString();
                movies = JsonConvert.DeserializeObject<List<MovieDto>>(extractedResult);
            }
            else
            {
                TempData["error"] = response?.Message ?? "Error retrieving movies";
            }

            var genreResponse = await _genreService.GetAllGenresAsync();
            if (genreResponse != null && genreResponse.IsSuccess)
            {
                var json = Convert.ToString(genreResponse.Result);
                var extractedResult = JObject.Parse(json)["result"].ToString();
                ViewBag.Genres = JsonConvert.DeserializeObject<List<GenreDto>>(extractedResult);
            }

            ViewBag.GenreId = genreId;
            return View(movies);
        }
    }
}
