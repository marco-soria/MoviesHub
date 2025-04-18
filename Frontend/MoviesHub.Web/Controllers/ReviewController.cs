using Microsoft.AspNetCore.Mvc;
using MoviesHub.Web.Models;
using MoviesHub.Web.Services.IServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MoviesHub.Web.Controllers
{
    public class ReviewController : Controller
    {
        private readonly IReviewService _reviewService;
        private readonly IMovieService _movieService;

        public ReviewController(IReviewService reviewService, IMovieService movieService)
        {
            _reviewService = reviewService;
            _movieService = movieService;
        }

        public async Task<IActionResult> Index()
        {
            List<ReviewDto> list = new();

            try
            {
                var response = await _reviewService.GetAllReviewsAsync();

                if (response != null && response.IsSuccess)
                {
                    // Implementar manejo adecuado de la respuesta JSON
                    if (response.Result != null)
                    {
                        // Convertir el objeto a JObject para poder inspeccionarlo
                        string jsonStr = JsonConvert.SerializeObject(response.Result);
                        Console.WriteLine($"Response JSON: {jsonStr}");

                        // Tratar de deserializar directamente
                        try
                        {
                            list = JsonConvert.DeserializeObject<List<ReviewDto>>(jsonStr);
                        }
                        catch
                        {
                            // Si falla, intentar extraer del objeto anidado
                            var jObj = JObject.Parse(jsonStr);
                            if (jObj.TryGetValue("result", out JToken resultToken))
                            {
                                list = JsonConvert.DeserializeObject<List<ReviewDto>>(resultToken.ToString());
                            }
                        }
                    }
                }
                else
                {
                    TempData["error"] = response?.Message ?? "Error al obtener las reseñas";
                }
            }
            catch (Exception ex)
            {
                TempData["error"] = $"Error: {ex.Message}";
                Console.WriteLine($"Exception: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }

            return View(list);
        }

        public async Task<IActionResult> Create()
        {
            try
            {
                // Obtener lista de películas para el menú desplegable
                var moviesResponse = await _movieService.GetAllMoviesAsync();

                if (moviesResponse != null && moviesResponse.IsSuccess && moviesResponse.Result != null)
                {
                    string jsonStr = JsonConvert.SerializeObject(moviesResponse.Result);

                    try
                    {
                        ViewBag.Movies = JsonConvert.DeserializeObject<List<MovieDto>>(jsonStr);
                    }
                    catch
                    {
                        var jObj = JObject.Parse(jsonStr);
                        if (jObj.TryGetValue("result", out JToken resultToken))
                        {
                            ViewBag.Movies = JsonConvert.DeserializeObject<List<MovieDto>>(resultToken.ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["error"] = $"Error al cargar películas: {ex.Message}";
                Console.WriteLine($"Exception in Create: {ex.Message}");
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ReviewCreateDto model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var response = await _reviewService.CreateReviewAsync(model);

                    if (response != null && response.IsSuccess)
                    {
                        TempData["success"] = "Reseña creada exitosamente";
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        TempData["error"] = response?.Message ?? "Error al crear la reseña";
                    }
                }
                catch (Exception ex)
                {
                    TempData["error"] = $"Error: {ex.Message}";
                }
            }

            // Repoblar la lista de películas en caso de error
            try
            {
                var moviesResponse = await _movieService.GetAllMoviesAsync();

                if (moviesResponse?.IsSuccess == true && moviesResponse.Result != null)
                {
                    string jsonStr = JsonConvert.SerializeObject(moviesResponse.Result);

                    try
                    {
                        ViewBag.Movies = JsonConvert.DeserializeObject<List<MovieDto>>(jsonStr);
                    }
                    catch
                    {
                        var jObj = JObject.Parse(jsonStr);
                        if (jObj.TryGetValue("result", out JToken resultToken))
                        {
                            ViewBag.Movies = JsonConvert.DeserializeObject<List<MovieDto>>(resultToken.ToString());
                        }
                    }
                }
            }
            catch { /* Ignorar errores aquí */ }

            return View(model);
        }

        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var response = await _reviewService.GetReviewByIdAsync(id);

                if (response?.IsSuccess == true && response.Result != null)
                {
                    ReviewDto review = null;
                    string jsonStr = JsonConvert.SerializeObject(response.Result);

                    try
                    {
                        review = JsonConvert.DeserializeObject<ReviewDto>(jsonStr);
                    }
                    catch
                    {
                        var jObj = JObject.Parse(jsonStr);
                        if (jObj.TryGetValue("result", out JToken resultToken))
                        {
                            review = JsonConvert.DeserializeObject<ReviewDto>(resultToken.ToString());
                        }
                    }

                    if (review != null)
                    {
                        ReviewUpdateDto model = new()
                        {
                            Comment = review.Comment,
                            Rating = review.Rating
                        };
                        return View(model);
                    }
                }

                TempData["error"] = "Reseña no encontrada";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["error"] = $"Error: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ReviewUpdateDto model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var response = await _reviewService.UpdateReviewAsync(id, model);

                    if (response?.IsSuccess == true)
                    {
                        TempData["success"] = "Reseña actualizada exitosamente";
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        TempData["error"] = response?.Message ?? "Error al actualizar la reseña";
                    }
                }
                catch (Exception ex)
                {
                    TempData["error"] = $"Error: {ex.Message}";
                }
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var response = await _reviewService.DeleteReviewAsync(id);

                if (response?.IsSuccess == true)
                {
                    TempData["success"] = "Reseña eliminada exitosamente";
                }
                else
                {
                    TempData["error"] = response?.Message ?? "Error al eliminar la reseña";
                }
            }
            catch (Exception ex)
            {
                TempData["error"] = $"Error: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> DeletedReviews()
        {
            List<ReviewDto> list = new();

            try
            {
                var response = await _reviewService.GetDeletedReviewsAsync();

                if (response?.IsSuccess == true && response.Result != null)
                {
                    string jsonStr = JsonConvert.SerializeObject(response.Result);

                    try
                    {
                        list = JsonConvert.DeserializeObject<List<ReviewDto>>(jsonStr);
                    }
                    catch
                    {
                        var jObj = JObject.Parse(jsonStr);
                        if (jObj.TryGetValue("result", out JToken resultToken))
                        {
                            list = JsonConvert.DeserializeObject<List<ReviewDto>>(resultToken.ToString());
                        }
                    }
                }
                else
                {
                    TempData["error"] = response?.Message ?? "Error al obtener las reseñas eliminadas";
                }
            }
            catch (Exception ex)
            {
                TempData["error"] = $"Error: {ex.Message}";
            }

            return View(list);
        }

        [HttpPost]
        public async Task<IActionResult> Restore(int id)
        {
            try
            {
                var response = await _reviewService.RestoreReviewAsync(id);

                if (response?.IsSuccess == true)
                {
                    TempData["success"] = "Reseña restaurada exitosamente";
                }
                else
                {
                    TempData["error"] = response?.Message ?? "Error al restaurar la reseña";
                }
            }
            catch (Exception ex)
            {
                TempData["error"] = $"Error: {ex.Message}";
            }

            return RedirectToAction(nameof(DeletedReviews));
        }
    }
}
