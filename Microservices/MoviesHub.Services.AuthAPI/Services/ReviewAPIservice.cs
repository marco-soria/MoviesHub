using MoviesHub.Services.AuthAPI.Models.Dto;
using MoviesHub.Services.AuthAPI.Services.IServices;
using Newtonsoft.Json;

namespace MoviesHub.Services.AuthAPI.Services
{
    public class ReviewAPIService : IReviewAPIService
    {
        private readonly HttpClient _httpClient;

        public ReviewAPIService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ReviewsAPI");
        }

        public async Task<T> GetUserReviewsAsync<T>(string userId)
        {
            var response = await _httpClient.GetAsync($"/api/reviews/user/{userId}");
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

        public async Task<T> GetMovieReviewStatsAsync<T>(int movieId)
        {
            var response = await _httpClient.GetAsync($"/api/reviews/movie/{movieId}/stats");
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
