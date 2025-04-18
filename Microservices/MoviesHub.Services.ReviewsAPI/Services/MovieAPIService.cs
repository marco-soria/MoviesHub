using MoviesHub.Services.ReviewsAPI.Models.Dto;
using MoviesHub.Services.ReviewsAPI.Services.IServices;
using Newtonsoft.Json;

namespace MoviesHub.Services.ReviewsAPI.Services
{
    public class MovieAPIService : IMovieAPIService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<MovieAPIService> _logger;

        public MovieAPIService(IHttpClientFactory httpClientFactory, ILogger<MovieAPIService> logger)
        {
            _httpClient = httpClientFactory.CreateClient("Movies");
            _logger = logger;
        }

        public async Task<bool> MovieExistsAsync(int movieId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/movies/{movieId}/exists");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var responseDto = JsonConvert.DeserializeObject<ResponseDto>(content);

                    if (responseDto != null && responseDto.IsSuccess && responseDto.Result != null)
                    {
                        return Convert.ToBoolean(responseDto.Result);
                    }
                }

                // Si la película no existe o hay error, asumimos que no existe
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if movie {MovieId} exists", movieId);
                return false; // En caso de error, asumimos que no existe
            }
        }

        public async Task<T> GetMovieDetailsAsync<T>(int movieId)
        {
            var response = await _httpClient.GetAsync($"/api/movies/{movieId}");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var responseDto = JsonConvert.DeserializeObject<ResponseDto>(content);
                if (responseDto != null && responseDto.IsSuccess)
                {
                    return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(responseDto.Result));
                }
            }
            return default;
        }

        public async Task<bool> NotifyRatingChangeAsync(int movieId)
        {
            try
            {
                var response = await _httpClient.PostAsync($"/api/movies/{movieId}/update-rating", null);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false; // Manejar fallo de manera silenciosa
            }
        }

    }
}
