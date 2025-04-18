using MoviesHub.Services.ReviewsAPI.Models.Dto;
using MoviesHub.Services.ReviewsAPI.Services.IServices;
using Newtonsoft.Json;

namespace MoviesHub.Services.ReviewsAPI.Services
{
    public class AuthAPIService : IAuthAPIService
    {
        private readonly HttpClient _httpClient;

        public AuthAPIService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("Auth");
        }

        public async Task<bool> ValidateTokenAsync(string token)
        {
            var requestData = new { Token = token };
            var response = await _httpClient.PostAsJsonAsync("/api/auth/validate", requestData);
            return response.IsSuccessStatusCode;
        }

        public async Task<T> GetUserDetailsAsync<T>(string userId)
        {
            var response = await _httpClient.GetAsync($"/api/users/{userId}");
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
