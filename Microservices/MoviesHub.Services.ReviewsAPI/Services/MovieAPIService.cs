using MoviesHub.Services.ReviewsAPI.Models.Dto;
using MoviesHub.Services.ReviewsAPI.Services.IServices;
using Newtonsoft.Json;

namespace MoviesHub.Services.ReviewsAPI.Services
{
    public class MovieAPIService : IMovieAPIService
    {
        private readonly HttpClient _httpClient;

        public MovieAPIService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("Movies");
        }

        public async Task<bool> MovieExistsAsync(int movieId)
        {
            var response = await _httpClient.GetAsync($"/api/movies/{movieId}/exists");
            return response.IsSuccessStatusCode;
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
    }
}
