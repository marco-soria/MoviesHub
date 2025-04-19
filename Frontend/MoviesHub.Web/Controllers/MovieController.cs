using Microsoft.AspNetCore.Mvc;
using MoviesHub.Web.Models;
using MoviesHub.Web.Services.IServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Security.Claims;
using Microsoft.Extensions.DependencyInjection;
using MoviesHub.Web.Utility;

namespace MoviesHub.Web.Controllers
{
    public class MovieController : Controller
    {
        private readonly IMovieService _movieService;
        private readonly IGenreService _genreService;
        private readonly IReviewService _reviewService;
        private readonly ILogger<MovieController> _logger;

        public MovieController(
            IMovieService movieService,
            IGenreService genreService,
            IReviewService reviewService,
            ILogger<MovieController> logger)
        {
            _movieService = movieService;
            _genreService = genreService;
            _reviewService = reviewService;
            _logger = logger;
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
            _logger.LogInformation($"Editando película con ID: {id}, Título: {model.Title}");

            if (ModelState.IsValid)
            {
                var response = await _movieService.UpdateMovieAsync(id, model);
                if (response != null && response.IsSuccess)
                {
                    TempData["success"] = "Película actualizada correctamente";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["error"] = response?.Message ?? "Error al actualizar la película";
                }
            }
            else
            {
                var errors = string.Join("; ", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));
                _logger.LogWarning($"ModelState inválido: {errors}");
            }

            // Si llegamos aquí, significa que hubo errores, necesitamos volver a obtener los géneros
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
            var viewModel = new MovieDetailsViewModel();

            try
            {
                // Obtener la película por ID
                var response = await _movieService.GetMovieByIdAsync(id);
                if (response != null && response.IsSuccess)
                {
                    string jsonStr = JsonConvert.SerializeObject(response.Result);
                    _logger.LogInformation($"Movie response: {jsonStr}");

                    try
                    {
                        // Intentar deserializar directamente
                        var jObj = JObject.Parse(jsonStr);
                        if (jObj.TryGetValue("result", out JToken resultToken))
                        {
                            viewModel.Movie = JsonConvert.DeserializeObject<MovieDto>(resultToken.ToString());
                        }
                        else
                        {
                            viewModel.Movie = JsonConvert.DeserializeObject<MovieDto>(jsonStr);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error deserializing movie data");
                        TempData["error"] = "Error procesando los datos de la película";
                        return RedirectToAction("Index", "Home");
                    }

                    // Obtener las reseñas para esta película
                    var reviewsResponse = await _reviewService.GetReviewsByMovieAsync(id);

                    if (reviewsResponse != null && reviewsResponse.IsSuccess)
                    {
                        string reviewsJson = JsonConvert.SerializeObject(reviewsResponse.Result);
                        _logger.LogInformation($"Reviews response: {reviewsJson}");

                        try
                        {
                            var jObj = JObject.Parse(reviewsJson);
                            if (jObj.TryGetValue("result", out JToken resultToken))
                            {
                                viewModel.Reviews = JsonConvert.DeserializeObject<List<ReviewDto>>(resultToken.ToString());
                            }
                            else
                            {
                                viewModel.Reviews = JsonConvert.DeserializeObject<List<ReviewDto>>(reviewsJson);
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error deserializing reviews data");
                            viewModel.Reviews = new List<ReviewDto>();
                        }
                    }

                    // Verificar si el usuario está autenticado
                    if (User.Identity?.IsAuthenticated == true)
                    {
                        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                        // Verificar si todos los claims están presentes
                        _logger.LogInformation("Claims disponibles:");
                        foreach (var claim in User.Claims)
                        {
                            _logger.LogInformation($"- {claim.Type}: {claim.Value}");
                        }

                        // Verificar que el userId no sea nulo o vacío
                        if (!string.IsNullOrEmpty(userId))
                        {
                            _logger.LogInformation($"Usuario autenticado con ID: {userId}");

                            // Agregar información de roles para la vista
                            ViewBag.IsAdmin = User.IsInRole(SD.RoleAdmin);
                            ViewBag.IsManager = User.IsInRole(SD.RoleManager);

                            viewModel.NewReview = new ReviewCreateDto
                            {
                                MovieId = id,
                                UserId = userId
                            };

                            // Verificar si el usuario ya tiene una reseña para esta película
                            var userReviewResponse = await _reviewService.GetUserReviewForMovieAsync(id, userId);

                            // Registro adicional para diagnosticar la respuesta
                            _logger.LogInformation($"Respuesta de GetUserReviewForMovieAsync: IsSuccess={userReviewResponse?.IsSuccess}, Message={userReviewResponse?.Message}");

                            if (userReviewResponse != null && userReviewResponse.IsSuccess)
                            {
                                try
                                {
                                    string userReviewJson = JsonConvert.SerializeObject(userReviewResponse.Result);
                                    _logger.LogInformation($"UserReview JSON: {userReviewJson}");

                                    var userReview = JsonConvert.DeserializeObject<ReviewDto>(userReviewJson);
                                    if (userReview != null)
                                    {
                                        ViewBag.UserReview = userReview;
                                        _logger.LogInformation($"Reseña del usuario encontrada para película {id}: {userReview.Id}");
                                    }
                                }
                                catch (Exception ex)
                                {
                                    _logger.LogError(ex, "Error deserializing user review data");
                                }
                            }

                            // Verificar si el usuario tiene reseñas eliminadas para esta película
                            try
                            {
                                var deletedReviewsResponse = await _reviewService.GetDeletedReviewsAsync();
                                if (deletedReviewsResponse != null && deletedReviewsResponse.IsSuccess)
                                {
                                    string deletedReviewsJson = JsonConvert.SerializeObject(deletedReviewsResponse.Result);
                                    List<ReviewDto> deletedReviews = null;

                                    try
                                    {
                                        var jObj = JObject.Parse(deletedReviewsJson);
                                        if (jObj.TryGetValue("result", out JToken resultToken))
                                        {
                                            deletedReviews = JsonConvert.DeserializeObject<List<ReviewDto>>(resultToken.ToString());
                                        }
                                        else
                                        {
                                            deletedReviews = JsonConvert.DeserializeObject<List<ReviewDto>>(deletedReviewsJson);
                                        }

                                        if (deletedReviews != null)
                                        {
                                            var userDeletedReview = deletedReviews.FirstOrDefault(r =>
                                                r.MovieId == id &&
                                                r.UserId == userId &&
                                                r.IsDeleted);

                                            ViewBag.UserHasDeletedReview = userDeletedReview != null;
                                            if (userDeletedReview != null)
                                            {
                                                ViewBag.UserDeletedReview = userDeletedReview;
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        _logger.LogError(ex, "Error deserializing deleted reviews data");
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, "Error checking for deleted reviews");
                            }
                        }
                        else
                        {
                            _logger.LogWarning("Usuario autenticado pero sin ID válido. JWT puede no tener claim 'sub'");
                        }
                    }
                }
                else
                {
                    TempData["error"] = response?.Message ?? "No se pudo encontrar la película";
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Details action");
                TempData["error"] = $"Error: {ex.Message}";
                return RedirectToAction("Index", "Home");
            }

            return View(viewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RateMovie(int movieId, int rating)
        {
            try
            {
                if (!User.Identity.IsAuthenticated)
                {
                    _logger.LogInformation("Unauthorized user attempted to rate movie");
                    return RedirectToAction("Login", "Auth", new { returnUrl = Url.Action("Details", new { id = movieId }) });
                }

                // Obtener el UserID desde Claims
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    TempData["error"] = "No se pudo identificar al usuario";
                    return RedirectToAction(nameof(Details), new { id = movieId });
                }

                // Crear un review mínimo con solo la calificación
                var createDto = new ReviewCreateDto
                {
                    MovieId = movieId,
                    UserId = userId,
                    Rating = rating,
                    Comment = "Sin comentario" // Comentario mínimo requerido
                };

                var response = await _reviewService.CreateReviewAsync(createDto);

                if (response != null)
                {
                    if (response.IsSuccess)
                    {
                        TempData["success"] = "Calificación guardada correctamente";
                    }
                    else
                    {
                        // Si hay error porque el usuario ya tiene una reseña, intentamos actualizar
                        if (response.Message?.Contains("already reviewed") == true)
                        {
                            // Verificar si el usuario tiene una reseña eliminada
                            var deletedReviewsResponse = await _reviewService.GetDeletedReviewsAsync();
                            bool hasDeletedReview = false;
                            ReviewDto deletedReview = null;

                            if (deletedReviewsResponse != null && deletedReviewsResponse.IsSuccess)
                            {
                                try
                                {
                                    string deletedReviewsJson = JsonConvert.SerializeObject(deletedReviewsResponse.Result);
                                    List<ReviewDto> allDeletedReviews = null;

                                    var jObj = JObject.Parse(deletedReviewsJson);
                                    if (jObj.TryGetValue("result", out JToken resultToken))
                                    {
                                        allDeletedReviews = JsonConvert.DeserializeObject<List<ReviewDto>>(resultToken.ToString());
                                    }
                                    else
                                    {
                                        allDeletedReviews = JsonConvert.DeserializeObject<List<ReviewDto>>(deletedReviewsJson);
                                    }

                                    if (allDeletedReviews != null)
                                    {
                                        deletedReview = allDeletedReviews.FirstOrDefault(r =>
                                            r.MovieId == movieId &&
                                            r.UserId == userId &&
                                            r.IsDeleted);

                                        hasDeletedReview = deletedReview != null;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    _logger.LogError(ex, "Error checking deleted reviews");
                                }
                            }

                            if (hasDeletedReview && deletedReview != null)
                            {
                                // Restaurar la reseña eliminada y actualizarla
                                var restoreResponse = await _reviewService.RestoreReviewAsync(deletedReview.Id);
                                if (restoreResponse != null && restoreResponse.IsSuccess)
                                {
                                    // Actualizar la reseña restaurada
                                    var updateDto = new ReviewUpdateDto
                                    {
                                        Comment = "Sin comentario",
                                        Rating = rating
                                    };

                                    var updateResponse = await _reviewService.UpdateReviewAsync(deletedReview.Id, updateDto);
                                    if (updateResponse != null && updateResponse.IsSuccess)
                                    {
                                        TempData["success"] = "Calificación restaurada y actualizada correctamente";
                                    }
                                    else
                                    {
                                        TempData["error"] = updateResponse?.Message ?? "Error al actualizar la calificación";
                                    }
                                }
                                else
                                {
                                    TempData["error"] = "Error al restaurar la reseña eliminada";
                                }
                            }
                            else
                            {
                                // Obtener la reseña existente
                                var userReviewResponse = await _reviewService.GetUserReviewForMovieAsync(movieId, userId);
                                if (userReviewResponse != null && userReviewResponse.IsSuccess)
                                {
                                    var userReview = JsonConvert.DeserializeObject<ReviewDto>(
                                        JsonConvert.SerializeObject(userReviewResponse.Result));

                                    if (userReview != null)
                                    {
                                        // Actualizar la calificación manteniendo el comentario
                                        var updateDto = new ReviewUpdateDto
                                        {
                                            Comment = userReview.Comment,
                                            Rating = rating
                                        };

                                        var updateResponse = await _reviewService.UpdateReviewAsync(userReview.Id, updateDto);
                                        if (updateResponse != null && updateResponse.IsSuccess)
                                        {
                                            TempData["success"] = "Calificación actualizada correctamente";
                                        }
                                        else
                                        {
                                            TempData["error"] = updateResponse?.Message ?? "Error al actualizar la calificación";
                                        }
                                    }
                                }
                                else
                                {
                                    TempData["error"] = "No se pudo actualizar la calificación existente";
                                }
                            }
                        }
                        else
                        {
                            TempData["error"] = response.Message ?? "Error al guardar la calificación";
                        }
                    }
                }
                else
                {
                    TempData["error"] = "Error de comunicación con el servicio de reseñas";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in RateMovie action");
                TempData["error"] = $"Error: {ex.Message}";
            }

            return RedirectToAction(nameof(Details), new { id = movieId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddReview(ReviewCreateDto model)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Auth", new { returnUrl = Url.Action("Details", new { id = model.MovieId }) });
            }

            try
            {
                // Obtener el UserID desde Claims
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    TempData["error"] = "No se pudo identificar al usuario";
                    return RedirectToAction(nameof(Details), new { id = model.MovieId });
                }

                // Asegurarse de que el userId sea correcto
                model.UserId = userId;

                if (ModelState.IsValid)
                {
                    _logger.LogInformation($"Creating review: {JsonConvert.SerializeObject(model)}");

                    // Verificar si el usuario tiene una reseña eliminada para esta película
                    var deletedReviewsResponse = await _reviewService.GetDeletedReviewsAsync();
                    ReviewDto deletedReview = null;
                    bool hasDeletedReview = false;

                    if (deletedReviewsResponse != null && deletedReviewsResponse.IsSuccess)
                    {
                        try
                        {
                            string deletedReviewsJson = JsonConvert.SerializeObject(deletedReviewsResponse.Result);
                            List<ReviewDto> allDeletedReviews = null;

                            var jObj = JObject.Parse(deletedReviewsJson);
                            if (jObj.TryGetValue("result", out JToken resultToken))
                            {
                                allDeletedReviews = JsonConvert.DeserializeObject<List<ReviewDto>>(resultToken.ToString());
                            }
                            else
                            {
                                allDeletedReviews = JsonConvert.DeserializeObject<List<ReviewDto>>(deletedReviewsJson);
                            }

                            if (allDeletedReviews != null)
                            {
                                deletedReview = allDeletedReviews.FirstOrDefault(r =>
                                    r.MovieId == model.MovieId &&
                                    r.UserId == userId &&
                                    r.IsDeleted);

                                hasDeletedReview = deletedReview != null;
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error checking deleted reviews");
                        }
                    }

                    // Si hay una reseña eliminada, restaurarla y actualizarla
                    if (hasDeletedReview && deletedReview != null)
                    {
                        var restoreResponse = await _reviewService.RestoreReviewAsync(deletedReview.Id);
                        if (restoreResponse != null && restoreResponse.IsSuccess)
                        {
                            // Actualizar la reseña restaurada
                            var updateDto = new ReviewUpdateDto
                            {
                                Comment = model.Comment,
                                Rating = model.Rating
                            };

                            var updateResponse = await _reviewService.UpdateReviewAsync(deletedReview.Id, updateDto);
                            if (updateResponse != null && updateResponse.IsSuccess)
                            {
                                TempData["success"] = "Reseña restaurada y actualizada correctamente";
                            }
                            else
                            {
                                TempData["error"] = updateResponse?.Message ?? "Error al actualizar la reseña";
                            }

                            return RedirectToAction(nameof(Details), new { id = model.MovieId });
                        }
                    }

                    // Si no hay reseña eliminada o no se pudo restaurar, intentar crear nueva
                    var response = await _reviewService.CreateReviewAsync(model);

                    if (response != null)
                    {
                        if (response.IsSuccess)
                        {
                            TempData["success"] = "Reseña publicada correctamente";
                        }
                        else
                        {
                            // Si hay error porque el usuario ya tiene una reseña, intentamos actualizar
                            if (response.Message?.Contains("already reviewed") == true)
                            {
                                // Obtener la reseña existente
                                var userReviewResponse = await _reviewService.GetUserReviewForMovieAsync(model.MovieId, userId);
                                if (userReviewResponse != null && userReviewResponse.IsSuccess)
                                {
                                    var userReview = JsonConvert.DeserializeObject<ReviewDto>(
                                        JsonConvert.SerializeObject(userReviewResponse.Result));

                                    if (userReview != null)
                                    {
                                        // Actualizar con los nuevos datos
                                        var updateDto = new ReviewUpdateDto
                                        {
                                            Comment = model.Comment,
                                            Rating = model.Rating
                                        };

                                        var updateResponse = await _reviewService.UpdateReviewAsync(userReview.Id, updateDto);
                                        if (updateResponse != null && updateResponse.IsSuccess)
                                        {
                                            TempData["success"] = "Reseña actualizada correctamente";
                                        }
                                        else
                                        {
                                            TempData["error"] = updateResponse?.Message ?? "Error al actualizar la reseña";
                                        }
                                    }
                                }
                                else
                                {
                                    TempData["error"] = "No se pudo actualizar la reseña existente";
                                }
                            }
                            else
                            {
                                TempData["error"] = response.Message ?? "Error al publicar la reseña";
                            }
                        }
                    }
                    else
                    {
                        TempData["error"] = "Error de comunicación con el servicio de reseñas";
                    }
                }
                else
                {
                    var errors = string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                    TempData["error"] = $"Por favor, completa correctamente todos los campos: {errors}";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in AddReview action");
                TempData["error"] = $"Error: {ex.Message}";
            }

            return RedirectToAction(nameof(Details), new { id = model.MovieId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateReview(int id, string comment, int rating, int movieId)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Auth");
            }

            try
            {
                // Get authenticated user ID
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                bool isAdmin = User.IsInRole(SD.RoleAdmin);
                bool isManager = User.IsInRole(SD.RoleManager);

                _logger.LogInformation($"Current user ID: {userId}, IsAdmin: {isAdmin}, IsManager: {isManager}");

                if (string.IsNullOrEmpty(userId) && !isAdmin && !isManager)
                {
                    TempData["error"] = "No se pudo identificar al usuario";
                    return RedirectToAction(nameof(Details), new { id = movieId });
                }

                // Get the review directly from API
                var getResponse = await _reviewService.GetReviewByIdAsync(id);
                if (getResponse == null || !getResponse.IsSuccess)
                {
                    TempData["error"] = "No se pudo obtener la reseña";
                    return RedirectToAction(nameof(Details), new { id = movieId });
                }

                // Log the raw response for debugging
                string responseJson = JsonConvert.SerializeObject(getResponse.Result);
                _logger.LogInformation($"Raw review data: {responseJson}");

                // Extract the review from the nested structure
                ReviewDto review = null;

                try
                {
                    // First try to parse the JSON object
                    JObject jObject = JObject.Parse(responseJson);

                    // Check if there's a "result" property
                    if (jObject.TryGetValue("result", out JToken resultToken))
                    {
                        // Deserialize from the result token
                        review = JsonConvert.DeserializeObject<ReviewDto>(resultToken.ToString());
                        _logger.LogInformation("Deserialized review from nested 'result' property");
                    }
                    else
                    {
                        // Try direct deserialization if no 'result' property
                        review = JsonConvert.DeserializeObject<ReviewDto>(responseJson);
                        _logger.LogInformation("Deserialized review directly from response");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Deserialization error: {ex.Message}");
                }

                // Final check if we got a valid review
                if (review == null)
                {
                    TempData["error"] = "Error procesando los datos de la reseña";
                    return RedirectToAction(nameof(Details), new { id = movieId });
                }

                // Log the review data and user IDs for comparison
                _logger.LogInformation($"Review data: MovieId={review.MovieId}, UserId={review.UserId}");
                _logger.LogInformation($"Current user: {userId}");

                // Authorization check: admin/manager can edit any review, users can only edit their own
                bool isAuthorized = isAdmin || isManager ||
                                   (String.Equals(review.UserId?.Trim(), userId?.Trim(), StringComparison.OrdinalIgnoreCase));

                _logger.LogInformation($"Authorization check: {isAuthorized}");

                if (!isAuthorized)
                {
                    _logger.LogWarning($"Authorization failed: Review UserId='{review.UserId}' doesn't match current UserId='{userId}' and user is not admin/manager");
                    TempData["error"] = "No tienes permisos para editar esta reseña";
                    return RedirectToAction(nameof(Details), new { id = review.MovieId > 0 ? review.MovieId : movieId });
                }

                // Update the review
                var updateDto = new ReviewUpdateDto
                {
                    Comment = comment,
                    Rating = rating
                };

                _logger.LogInformation($"Updating review {id} with data: {JsonConvert.SerializeObject(updateDto)}");
                var response = await _reviewService.UpdateReviewAsync(id, updateDto);

                if (response?.IsSuccess == true)
                {
                    TempData["success"] = "Reseña actualizada correctamente";
                }
                else
                {
                    TempData["error"] = response?.Message ?? "Error al actualizar la reseña";
                }

                // Go to movie details
                return RedirectToAction(nameof(Details), new { id = review.MovieId > 0 ? review.MovieId : movieId });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in UpdateReview: {ex.Message}");
                TempData["error"] = $"Error: {ex.Message}";
                return RedirectToAction(nameof(Details), new { id = movieId });
            }
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteReview(int reviewId)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Auth");
            }

            try
            {
                // Get user ID and role information
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                bool isAdmin = User.IsInRole(SD.RoleAdmin);
                bool isManager = User.IsInRole(SD.RoleManager);

                if (string.IsNullOrEmpty(userId) && !isAdmin && !isManager)
                {
                    _logger.LogWarning("User ID not found in claims and user is not admin/manager");
                    TempData["error"] = "No se pudo identificar al usuario";
                    return RedirectToAction("Index", "Home");
                }

                _logger.LogInformation($"Authenticated user ID for delete: {userId}, IsAdmin: {isAdmin}, IsManager: {isManager}");

                // Get the review
                var reviewService = HttpContext.RequestServices.GetRequiredService<IReviewService>();
                var getResponse = await reviewService.GetReviewByIdAsync(reviewId);

                if (getResponse == null || !getResponse.IsSuccess)
                {
                    TempData["error"] = getResponse?.Message ?? "No se encontró la reseña";
                    return RedirectToAction("Index", "Home");
                }

                // Extract the review data
                string jsonStr = JsonConvert.SerializeObject(getResponse.Result);
                _logger.LogInformation($"Review to delete: {jsonStr}");

                ReviewDto review = null;
                try
                {
                    // First try to parse the JSON object
                    JObject jObject = JObject.Parse(jsonStr);

                    // Check if there's a "result" property
                    if (jObject.TryGetValue("result", out JToken resultToken))
                    {
                        // Deserialize from the result token
                        review = JsonConvert.DeserializeObject<ReviewDto>(resultToken.ToString());
                    }
                    else
                    {
                        // Try direct deserialization if no 'result' property
                        review = JsonConvert.DeserializeObject<ReviewDto>(jsonStr);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error deserializing review data: {jsonStr}");
                }

                if (review == null)
                {
                    TempData["error"] = "Error al procesar los datos de la reseña";
                    return RedirectToAction("Index", "Home");
                }

                int movieId = review.MovieId;

                // Log the user IDs for comparison
                _logger.LogInformation($"Review's user ID: '{review.UserId}'");
                _logger.LogInformation($"Current user ID: '{userId}'");

                // Authorization check: admin/manager can delete any review, users can only delete their own
                bool isAuthorized = isAdmin || isManager ||
                                    (!string.IsNullOrEmpty(review.UserId) &&
                                     !string.IsNullOrEmpty(userId) &&
                                     review.UserId.Trim().Equals(userId.Trim(), StringComparison.OrdinalIgnoreCase));

                if (!isAuthorized)
                {
                    TempData["error"] = "No tienes permisos para eliminar esta reseña";
                    return RedirectToAction(nameof(Details), new { id = movieId });
                }

                // Delete the review
                var response = await reviewService.DeleteReviewAsync(reviewId);
                if (response != null && response.IsSuccess)
                {
                    TempData["success"] = "Reseña eliminada correctamente";
                }
                else
                {
                    TempData["error"] = response?.Message ?? "Error al eliminar la reseña";
                }

                return RedirectToAction(nameof(Details), new { id = movieId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in DeleteReview action");
                TempData["error"] = $"Error: {ex.Message}";
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RestoreReview(int reviewId)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Auth");
            }

            try
            {
                // Get user ID and role information
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                bool isAdmin = User.IsInRole(SD.RoleAdmin);
                bool isManager = User.IsInRole(SD.RoleManager);

                if (string.IsNullOrEmpty(userId) && !isAdmin && !isManager)
                {
                    _logger.LogWarning("User ID not found in claims and user is not admin/manager");
                    TempData["error"] = "No se pudo identificar al usuario";
                    return RedirectToAction("Index", "Home");
                }

                // Get the deleted review
                var deletedReviewsResponse = await _reviewService.GetDeletedReviewsAsync();

                if (deletedReviewsResponse == null || !deletedReviewsResponse.IsSuccess)
                {
                    TempData["error"] = "No se pudieron obtener las reseñas eliminadas";
                    return RedirectToAction("Index", "Home");
                }

                // Extract all deleted reviews
                string jsonStr = JsonConvert.SerializeObject(deletedReviewsResponse.Result);
                List<ReviewDto> deletedReviews = null;

                try
                {
                    var jObj = JObject.Parse(jsonStr);
                    if (jObj.TryGetValue("result", out JToken resultToken))
                    {
                        deletedReviews = JsonConvert.DeserializeObject<List<ReviewDto>>(resultToken.ToString());
                    }
                    else
                    {
                        deletedReviews = JsonConvert.DeserializeObject<List<ReviewDto>>(jsonStr);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error deserializing deleted reviews data: {jsonStr}");
                }

                if (deletedReviews == null)
                {
                    TempData["error"] = "Error al procesar los datos de las reseñas eliminadas";
                    return RedirectToAction("Index", "Home");
                }

                // Find the specific review
                var deletedReview = deletedReviews.FirstOrDefault(r => r.Id == reviewId);

                if (deletedReview == null)
                {
                    TempData["error"] = "No se encontró la reseña eliminada";
                    return RedirectToAction("Index", "Home");
                }

                int movieId = deletedReview.MovieId;

                // Authorization check
                bool isAuthorized = isAdmin || isManager ||
                                    (!string.IsNullOrEmpty(deletedReview.UserId) &&
                                     !string.IsNullOrEmpty(userId) &&
                                     deletedReview.UserId.Trim().Equals(userId.Trim(), StringComparison.OrdinalIgnoreCase));

                if (!isAuthorized)
                {
                    TempData["error"] = "No tienes permisos para restaurar esta reseña";
                    return RedirectToAction(nameof(Details), new { id = movieId });
                }

                // Restore the review
                var response = await _reviewService.RestoreReviewAsync(reviewId);
                if (response != null && response.IsSuccess)
                {
                    TempData["success"] = "Reseña restaurada correctamente";
                }
                else
                {
                    TempData["error"] = response?.Message ?? "Error al restaurar la reseña";
                }

                return RedirectToAction(nameof(Details), new { id = movieId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in RestoreReview action");
                TempData["error"] = $"Error: {ex.Message}";
                return RedirectToAction("Index", "Home");
            }
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

        public async Task<IActionResult> DeletedReviews()
        {
            // Solo administradores y managers pueden ver reseñas eliminadas
            if (!(User.IsInRole(SD.RoleAdmin) || User.IsInRole(SD.RoleManager)))
            {
                return RedirectToAction("AccessDenied", "Auth");
            }

            var deletedReviewsResponse = await _reviewService.GetDeletedReviewsAsync();
            List<ReviewDto> deletedReviews = new();

            if (deletedReviewsResponse != null && deletedReviewsResponse.IsSuccess)
            {
                string jsonStr = JsonConvert.SerializeObject(deletedReviewsResponse.Result);

                try
                {
                    var jObj = JObject.Parse(jsonStr);
                    if (jObj.TryGetValue("result", out JToken resultToken))
                    {
                        deletedReviews = JsonConvert.DeserializeObject<List<ReviewDto>>(resultToken.ToString());
                    }
                    else
                    {
                        deletedReviews = JsonConvert.DeserializeObject<List<ReviewDto>>(jsonStr);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error deserializing deleted reviews data");
                }
            }
            else
            {
                TempData["error"] = deletedReviewsResponse?.Message ?? "Error al obtener las reseñas eliminadas";
            }

            return View(deletedReviews);
        }

        [HttpPost]
        public IActionResult UpdateReviewDebug(int id, string comment, int rating, int movieId, string userId)
        {
            _logger.LogInformation("Debug form submitted with: " +
                $"id={id}, comment='{comment}', rating={rating}, movieId={movieId}, userId='{userId}'");

            // Log all available claims
            _logger.LogInformation("All claims:");
            foreach (var claim in User.Claims)
            {
                _logger.LogInformation($"- {claim.Type}: {claim.Value}");
            }

            return RedirectToAction(nameof(Details), new { id = movieId });
        }
    }
}
