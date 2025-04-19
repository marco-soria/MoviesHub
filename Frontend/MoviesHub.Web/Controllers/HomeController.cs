using Microsoft.AspNetCore.Mvc;
using MoviesHub.Web.Models;
using MoviesHub.Web.Services.IServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Security.Claims;
using Microsoft.Extensions.DependencyInjection;

namespace MoviesHub.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IMovieService _movieService;

        public HomeController(ILogger<HomeController> logger, IMovieService movieService)
        {
            _logger = logger;
            _movieService = movieService;
        }

        public async Task<IActionResult> Index()
        {
            List<MovieDto> movies = new();
            try
            {
                var response = await _movieService.GetAllMoviesAsync();
                _logger.LogInformation($"API Response for movies: {JsonConvert.SerializeObject(response)}");

                if (response != null && response.IsSuccess)
                {
                    string jsonStr = JsonConvert.SerializeObject(response.Result);
                    _logger.LogInformation($"Result content: {jsonStr}");

                    try
                    {
                        // Intentar extraer del objeto anidado primero
                        var jObj = JObject.Parse(jsonStr);
                        if (jObj.TryGetValue("result", out JToken resultToken))
                        {
                            movies = JsonConvert.DeserializeObject<List<MovieDto>>(resultToken.ToString());
                            _logger.LogInformation($"Successfully deserialized nested result with {movies.Count} movies");
                        }
                        else
                        {
                            // Si no hay objeto anidado, intentar deserializar directamente
                            movies = JsonConvert.DeserializeObject<List<MovieDto>>(jsonStr);
                            _logger.LogInformation($"Successfully deserialized direct result with {movies.Count} movies");
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error deserializing movies");
                    }
                }
                else
                {
                    _logger.LogWarning($"API returned unsuccessful response: {response?.Message}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching movies");
            }

            return View(movies ?? new List<MovieDto>());
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
