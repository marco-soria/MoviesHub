using Microsoft.AspNetCore.Mvc;
using MoviesHub.Web.Models;
using MoviesHub.Web.Service.IService;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MoviesHub.Web.Controllers
{
    public class GenreController : Controller
    {
        private readonly IGenreService _genreService;

        public GenreController(IGenreService genreService)
        {
            _genreService = genreService;
        }

        public async Task<IActionResult> Index()
        {
            var response = await _genreService.GetAllGenresAsync();
            List<GenreDto>? genres = new();

            if (response != null && response.IsSuccess)
            {
                //genres = JsonConvert.DeserializeObject<List<GenreDto>>(Convert.ToString(response.Result));
                var json = Convert.ToString(response.Result);
                var extractedResult = JObject.Parse(json)["result"].ToString();
                genres = JsonConvert.DeserializeObject<List<GenreDto>>(extractedResult);
            }
            else
            {
                TempData["error"] = response?.Message ?? "Error retrieving genres";
            }

            return View(genres);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(GenreCreateDto model)
        {
            if (ModelState.IsValid)
            {
                var response = await _genreService.CreateGenreAsync(model);
                if (response != null && response.IsSuccess)
                {
                    TempData["success"] = "Genre created successfully";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["error"] = response?.Message ?? "Error creating genre";
                }
            }
            return View(model);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var response = await _genreService.GetGenreByIdAsync(id);
            if (response != null && response.IsSuccess)
            {
                GenreDto genre = JsonConvert.DeserializeObject<GenreDto>(Convert.ToString(response.Result));
                GenreUpdateDto updateDto = new() { Name = genre.Name };
                return View(updateDto);
            }
            TempData["error"] = response?.Message ?? "Error retrieving genre";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, GenreUpdateDto model)
        {
            if (ModelState.IsValid)
            {
                var response = await _genreService.UpdateGenreAsync(id, model);
                if (response != null && response.IsSuccess)
                {
                    TempData["success"] = "Genre updated successfully";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["error"] = response?.Message ?? "Error updating genre";
                }
            }
            return View(model);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var response = await _genreService.GetGenreByIdAsync(id);
            if (response != null && response.IsSuccess)
            {
                GenreDto genre = JsonConvert.DeserializeObject<GenreDto>(Convert.ToString(response.Result));
                return View(genre);
            }
            TempData["error"] = response?.Message ?? "Error retrieving genre";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var response = await _genreService.DeleteGenreAsync(id);
            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Genre deleted successfully";
            }
            else
            {
                TempData["error"] = response?.Message ?? "Error deleting genre";
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Restore(int id)
        {
            var response = await _genreService.RestoreGenreAsync(id);
            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Genre restored successfully";
            }
            else
            {
                TempData["error"] = response?.Message ?? "Error restoring genre";
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Movies(int id)
        {
            var response = await _genreService.GetMoviesForGenreAsync(id);
            List<MovieDto>? movies = new();

            if (response != null && response.IsSuccess)
            {
                movies = JsonConvert.DeserializeObject<List<MovieDto>>(Convert.ToString(response.Result));
            }
            else
            {
                TempData["error"] = response?.Message ?? "Error retrieving movies for genre";
            }

            ViewBag.GenreId = id;
            var genreResponse = await _genreService.GetGenreByIdAsync(id);
            if (genreResponse != null && genreResponse.IsSuccess)
            {
                GenreDto genre = JsonConvert.DeserializeObject<GenreDto>(Convert.ToString(genreResponse.Result));
                ViewBag.GenreName = genre.Name;
            }

            return View(movies);
        }

        public async Task<IActionResult> GetGenresForMovies()
        {
            var response = await _genreService.GetAllGenresAsync();
            List<GenreDto>? genres = new();

            if (response != null && response.IsSuccess)
            {
                var json = Convert.ToString(response.Result);
                var extractedResult = JObject.Parse(json)["result"].ToString();
                genres = JsonConvert.DeserializeObject<List<GenreDto>>(extractedResult);
            }
            else
            {
                TempData["error"] = response?.Message ?? "Error retrieving genres";
            }

            return Json(genres);
        }
    }
}
