using Microsoft.AspNetCore.Mvc;
using MoviesHub.Web.Models;
using MoviesHub.Web.Services.IServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

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
                try
                {
                    // Convert the result to a string
                    var json = Convert.ToString(response.Result);
                    
                    // Parse the JSON and extract the nested "result" array
                    var extractedResult = JObject.Parse(json)["result"].ToString();
                    
                    // Deserialize the extracted array into a list of MovieGenreDto
                    movieGenres = JsonConvert.DeserializeObject<List<MovieGenreDto>>(extractedResult);
                }
                catch (Exception ex)
                {
                    TempData["error"] = $"Error parsing movie-genre data: {ex.Message}";
                    Console.WriteLine($"JSON Parsing Error: {ex.Message}");
                    Console.WriteLine($"Response Result: {JsonConvert.SerializeObject(response.Result)}");
                }
            }
            else
            {
                TempData["error"] = response?.Message ?? "Error retrieving movie-genre associations";
            }

            return View(movieGenres ?? new List<MovieGenreDto>());
        }

        public async Task<IActionResult> Create()
        {
            // Get all movies and genres for selection
            var movieResponse = await _movieService.GetAllMoviesAsync();
            var genreResponse = await _genreService.GetAllGenresAsync();

            if (movieResponse != null && movieResponse.IsSuccess)
            {
                try
                {
                    var json = Convert.ToString(movieResponse.Result);
                    var extractedResult = JObject.Parse(json)["result"].ToString();
                    ViewBag.Movies = JsonConvert.DeserializeObject<List<MovieDto>>(extractedResult);
                }
                catch (Exception)
                {
                    ViewBag.Movies = new List<MovieDto>();
                    TempData["error"] = "Error parsing movie data";
                }
            }
            else
            {
                ViewBag.Movies = new List<MovieDto>();
                TempData["error"] = movieResponse?.Message ?? "Error retrieving movies";
            }

            if (genreResponse != null && genreResponse.IsSuccess)
            {
                try
                {
                    var json = Convert.ToString(genreResponse.Result);
                    var extractedResult = JObject.Parse(json)["result"].ToString();
                    ViewBag.Genres = JsonConvert.DeserializeObject<List<GenreDto>>(extractedResult);
                }
                catch (Exception)
                {
                    ViewBag.Genres = new List<GenreDto>();
                    TempData["error"] = "Error parsing genre data";
                }
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
                try
                {
                    var json = Convert.ToString(movieResponse.Result);
                    var extractedResult = JObject.Parse(json)["result"].ToString();
                    ViewBag.Movies = JsonConvert.DeserializeObject<List<MovieDto>>(extractedResult);
                }
                catch (Exception)
                {
                    ViewBag.Movies = new List<MovieDto>();
                }
            }
            else
            {
                ViewBag.Movies = new List<MovieDto>();
            }

            if (genreResponse != null && genreResponse.IsSuccess)
            {
                try
                {
                    var json = Convert.ToString(genreResponse.Result);
                    var extractedResult = JObject.Parse(json)["result"].ToString();
                    ViewBag.Genres = JsonConvert.DeserializeObject<List<GenreDto>>(extractedResult);
                }
                catch (Exception)
                {
                    ViewBag.Genres = new List<GenreDto>();
                }
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
                try
                {
                    var json = Convert.ToString(response.Result);
                    var extractedResult = JObject.Parse(json)["result"].ToString();
                    MovieGenreDto movieGenre = JsonConvert.DeserializeObject<MovieGenreDto>(extractedResult);
                    return View(movieGenre);
                }
                catch (Exception)
                {
                    TempData["error"] = "Error parsing movie-genre data";
                    return RedirectToAction(nameof(Index));
                }
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
                try
                {
                    var json = Convert.ToString(response.Result);
                    var extractedResult = JObject.Parse(json)["result"].ToString();
                    genres = JsonConvert.DeserializeObject<List<GenreDto>>(extractedResult);
                }
                catch (Exception ex)
                {
                    TempData["error"] = $"Error parsing genre data: {ex.Message}";
                }
            }
            else
            {
                TempData["error"] = response?.Message ?? "Error retrieving genres for movie";
            }

            // Get movie details for display
            var movieResponse = await _movieService.GetMovieByIdAsync(movieId);
            if (movieResponse != null && movieResponse.IsSuccess)
            {
                try
                {
                    var json = Convert.ToString(movieResponse.Result);
                    var extractedResult = JObject.Parse(json)["result"].ToString();
                    MovieDto movie = JsonConvert.DeserializeObject<MovieDto>(extractedResult);
                    ViewBag.MovieTitle = movie.Title;
                }
                catch (Exception)
                {
                    ViewBag.MovieTitle = "Unknown Movie";
                }
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
                try
                {
                    var json = Convert.ToString(response.Result);
                    var extractedResult = JObject.Parse(json)["result"].ToString();
                    movies = JsonConvert.DeserializeObject<List<MovieDto>>(extractedResult);
                }
                catch (Exception ex)
                {
                    TempData["error"] = $"Error parsing movie data: {ex.Message}";
                }
            }
            else
            {
                TempData["error"] = response?.Message ?? "Error retrieving movies for genre";
            }

            // Get genre details for display
            var genreResponse = await _genreService.GetGenreByIdAsync(genreId);
            if (genreResponse != null && genreResponse.IsSuccess)
            {
                try
                {
                    var json = Convert.ToString(genreResponse.Result);
                    var extractedResult = JObject.Parse(json)["result"].ToString();
                    GenreDto genre = JsonConvert.DeserializeObject<GenreDto>(extractedResult);
                    ViewBag.GenreName = genre.Name;
                }
                catch (Exception)
                {
                    ViewBag.GenreName = "Unknown Genre";
                }
            }

            ViewBag.GenreId = genreId;
            return View(movies);
        }
    }
}