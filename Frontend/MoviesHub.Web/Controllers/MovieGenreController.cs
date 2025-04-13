using Microsoft.AspNetCore.Mvc;
using MoviesHub.Web.Models;
using MoviesHub.Web.Service.IService;
using Newtonsoft.Json;

namespace MoviesHub.Web.Controllers
{
    public class MovieGenreController : Controller
    {
        private readonly IMovieGenreService _movieGenreService;
        private readonly IMovieService _movieService;
        private readonly IGenreService _genreService;

        public MovieGenreController(IMovieGenreService movieGenreService, IMovieService movieService, IGenreService genreService)
        {
            _movieGenreService = movieGenreService;
            _movieService = movieService;
            _genreService = genreService;
        }

        public async Task<IActionResult> Index()
        {
            var response = await _movieGenreService.GetAllMovieGenresAsync();
            List<MovieGenreDto>? movieGenres = new();

            if (response != null && response.IsSuccess)
            {
                movieGenres = JsonConvert.DeserializeObject<List<MovieGenreDto>>(Convert.ToString(response.Result));
            }
            else
            {
                TempData["error"] = response?.Message ?? "Error retrieving movie-genre associations";
            }

            return View(movieGenres);
        }

        public async Task<IActionResult> Create()
        {
            // Get all movies and genres for selection
            var movieResponse = await _movieService.GetAllMoviesAsync();
            var genreResponse = await _genreService.GetAllGenresAsync();

            if (movieResponse != null && movieResponse.IsSuccess)
            {
                ViewBag.Movies = JsonConvert.DeserializeObject<List<MovieDto>>(Convert.ToString(movieResponse.Result));
            }
            else
            {
                ViewBag.Movies = new List<MovieDto>();
                TempData["error"] = movieResponse?.Message ?? "Error retrieving movies";
            }

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
        public async Task<IActionResult> Create(MovieGenreCreateDto model)
        {
            if (ModelState.IsValid)
            {
                var response = await _movieGenreService.CreateMovieGenreAsync(model);
                if (response != null && response.IsSuccess)
                {
                    TempData["success"] = "Movie-Genre association created successfully";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["error"] = response?.Message ?? "Error creating movie-genre association";
                }
            }

            // Repopulate selections if validation fails
            var movieResponse = await _movieService.GetAllMoviesAsync();
            var genreResponse = await _genreService.GetAllGenresAsync();

            if (movieResponse != null && movieResponse.IsSuccess)
            {
                ViewBag.Movies = JsonConvert.DeserializeObject<List<MovieDto>>(Convert.ToString(movieResponse.Result));
            }
            else
            {
                ViewBag.Movies = new List<MovieDto>();
            }

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

        public async Task<IActionResult> Delete(int movieId, int genreId)
        {
            var response = await _movieGenreService.GetMovieGenreAsync(movieId, genreId);
            if (response != null && response.IsSuccess)
            {
                MovieGenreDto movieGenre = JsonConvert.DeserializeObject<MovieGenreDto>(Convert.ToString(response.Result));
                return View(movieGenre);
            }

            TempData["error"] = response?.Message ?? "Error retrieving movie-genre association";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int movieId, int genreId)
        {
            var response = await _movieGenreService.DeleteMovieGenreAsync(movieId, genreId);
            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Movie-Genre association deleted successfully";
            }
            else
            {
                TempData["error"] = response?.Message ?? "Error deleting movie-genre association";
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> MovieGenres(int movieId)
        {
            var response = await _movieGenreService.GetGenresForMovieAsync(movieId);
            List<GenreDto>? genres = new();

            if (response != null && response.IsSuccess)
            {
                genres = JsonConvert.DeserializeObject<List<GenreDto>>(Convert.ToString(response.Result));
            }
            else
            {
                TempData["error"] = response?.Message ?? "Error retrieving genres for movie";
            }

            // Get movie details for display
            var movieResponse = await _movieService.GetMovieByIdAsync(movieId);
            if (movieResponse != null && movieResponse.IsSuccess)
            {
                MovieDto movie = JsonConvert.DeserializeObject<MovieDto>(Convert.ToString(movieResponse.Result));
                ViewBag.MovieTitle = movie.Title;
            }

            ViewBag.MovieId = movieId;
            return View(genres);
        }

        public async Task<IActionResult> GenreMovies(int genreId)
        {
            var response = await _movieGenreService.GetMoviesForGenreAsync(genreId);
            List<MovieDto>? movies = new();

            if (response != null && response.IsSuccess)
            {
                movies = JsonConvert.DeserializeObject<List<MovieDto>>(Convert.ToString(response.Result));
            }
            else
            {
                TempData["error"] = response?.Message ?? "Error retrieving movies for genre";
            }

            // Get genre details for display
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
