using MoviesHub.Web.Models;
using MoviesHub.Web.Services.IServices;
using MoviesHub.Web.Utility;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static MoviesHub.Web.Utility.SD;

namespace MoviesHub.Web.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IBaseService _baseService;
        private readonly ILogger<ReviewService> _logger;

        public ReviewService(IBaseService baseService, ILogger<ReviewService> logger)
        {
            _baseService = baseService;
            _logger = logger;
        }

        public async Task<ResponseDto> GetAllReviewsAsync()
        {
            return await _baseService.SendAsync(new RequestDto
            {
                ApiType = ApiType.GET,
                Url = $"{ReviewAPIBase}/api/reviews"
            });
        }

        public async Task<ResponseDto> GetReviewByIdAsync(int id)
        {
            return await _baseService.SendAsync(new RequestDto
            {
                ApiType = ApiType.GET,
                Url = $"{ReviewAPIBase}/api/reviews/{id}"
            });
        }

        public async Task<ResponseDto> GetReviewsByMovieAsync(int movieId)
        {
            return await _baseService.SendAsync(new RequestDto
            {
                ApiType = ApiType.GET,
                Url = $"{ReviewAPIBase}/api/reviews/movie/{movieId}"
            });
        }

        public async Task<T> GetReviewsByMovieAsync<T>(int movieId)
        {
            var response = await _baseService.SendAsync(new RequestDto
            {
                ApiType = ApiType.GET,
                Url = $"{ReviewAPIBase}/api/reviews/movie/{movieId}"
            });

            if (response != null && response.IsSuccess)
            {
                return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(response.Result));
            }
            return default;
        }

        public async Task<ResponseDto> GetReviewsByUserAsync(string userId)
        {
            // Validar que el userId no sea nulo o vacío
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("GetReviewsByUserAsync called with null or empty userId");
                return new ResponseDto
                {
                    IsSuccess = false,
                    Message = "El ID de usuario es necesario para obtener las reseñas"
                };
            }

            return await _baseService.SendAsync(new RequestDto
            {
                ApiType = ApiType.GET,
                Url = $"{ReviewAPIBase}/api/reviews/user/{userId}"
            });
        }

        public async Task<ResponseDto> CreateReviewAsync(ReviewCreateDto review)
        {
            // Validar que el userId no sea nulo o vacío
            if (string.IsNullOrEmpty(review.UserId))
            {
                _logger.LogWarning("CreateReviewAsync called with null or empty userId");
                return new ResponseDto
                {
                    IsSuccess = false,
                    Message = "El ID de usuario es necesario para crear una reseña"
                };
            }

            return await _baseService.SendAsync(new RequestDto
            {
                ApiType = ApiType.POST,
                Data = review,
                Url = $"{ReviewAPIBase}/api/reviews"
            });
        }

        public async Task<ResponseDto> UpdateReviewAsync(int id, ReviewUpdateDto review)
        {
            return await _baseService.SendAsync(new RequestDto
            {
                ApiType = ApiType.PUT,
                Data = review,
                Url = $"{ReviewAPIBase}/api/reviews/{id}"
            });
        }

        public async Task<ResponseDto> DeleteReviewAsync(int id)
        {
            return await _baseService.SendAsync(new RequestDto
            {
                ApiType = ApiType.DELETE,
                Url = $"{ReviewAPIBase}/api/reviews/{id}"
            });
        }

        public async Task<ResponseDto> RestoreReviewAsync(int id)
        {
            return await _baseService.SendAsync(new RequestDto
            {
                ApiType = ApiType.PATCH,
                Url = $"{ReviewAPIBase}/api/reviews/{id}/restore"
            });
        }

        public async Task<ResponseDto> GetDeletedReviewsAsync()
        {
            return await _baseService.SendAsync(new RequestDto
            {
                ApiType = ApiType.GET,
                Url = $"{ReviewAPIBase}/api/reviews/deleted"
            });
        }

        public async Task<ResponseDto> GetUserReviewForMovieAsync(int movieId, string userId)
        {
            // Validar que el userId no sea nulo o vacío
            if (string.IsNullOrEmpty(userId))
            {
                return new ResponseDto
                {
                    IsSuccess = false,
                    Message = "El ID de usuario es requerido para obtener la reseña"
                };
            }

            try
            {
                // Obtenemos las reseñas del usuario y filtramos por movieId
                var response = await GetReviewsByUserAsync(userId);

                if (response != null && response.IsSuccess && response.Result != null)
                {
                    // Verificamos si el resultado es un array o un objeto único
                    var resultType = response.Result.GetType();
                    List<ReviewDto> reviews = null;

                    if (response.Result is JArray || response.Result is IEnumerable<ReviewDto>)
                    {
                        // Si es un array, deserializamos como lista
                        reviews = JsonConvert.DeserializeObject<List<ReviewDto>>(
                            JsonConvert.SerializeObject(response.Result));
                    }
                    else if (response.Result is JObject)
                    {
                        // Si es un objeto, verificamos si contiene una propiedad "result" que sea array
                        JObject resultObj = (JObject)response.Result;
                        if (resultObj["result"] != null && resultObj["result"] is JArray)
                        {
                            reviews = JsonConvert.DeserializeObject<List<ReviewDto>>(
                                resultObj["result"].ToString());
                        }
                        else
                        {
                            // Si no hay array, podría ser una única reseña
                            var singleReview = JsonConvert.DeserializeObject<ReviewDto>(
                                JsonConvert.SerializeObject(response.Result));

                            if (singleReview != null)
                            {
                                reviews = new List<ReviewDto> { singleReview };
                            }
                        }
                    }

                    if (reviews != null)
                    {
                        var userReview = reviews.FirstOrDefault(r => r.MovieId == movieId && !r.IsDeleted);

                        if (userReview != null)
                        {
                            var result = new ResponseDto
                            {
                                IsSuccess = true,
                                Result = userReview
                            };
                            return result;
                        }
                    }
                }

                return new ResponseDto
                {
                    IsSuccess = false,
                    Message = "No se encontró ninguna reseña del usuario para esta película"
                };
            }
            catch (Exception ex)
            {
                return new ResponseDto
                {
                    IsSuccess = false,
                    Message = $"Error al procesar las reseñas: {ex.Message}"
                };
            }
        }

    }
}
