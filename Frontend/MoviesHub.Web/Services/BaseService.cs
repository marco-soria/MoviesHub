using MoviesHub.Web.Models;
using MoviesHub.Web.Services.IServices;
using Newtonsoft.Json;
using static MoviesHub.Web.Utility.SD;
using System.Net;
using System.Text;

namespace MoviesHub.Web.Services
{
    public class BaseService : IBaseService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ITokenProvider _tokenProvider;

        public BaseService(IHttpClientFactory httpClientFactory, ITokenProvider tokenProvider)
        {
            _httpClientFactory = httpClientFactory;
            _tokenProvider = tokenProvider;
        }

        public async Task<ResponseDto> SendAsync(RequestDto requestDto, bool withBearerToken = true)
        {
            try
            {
                HttpClient client = _httpClientFactory.CreateClient("MoviesHubAPI");
                HttpRequestMessage message = new HttpRequestMessage();

                // Add headers
                message.Headers.Add("Accept", "application/json");

                // Add bearer token if required
                if (withBearerToken)
                {
                    var token = _tokenProvider.GetToken();
                    message.Headers.Add("Authorization", $"Bearer {token}");
                }

                // Set request URI and method
                message.RequestUri = new Uri(requestDto.Url);
                message.Method = requestDto.ApiType switch
                {
                    ApiType.GET => HttpMethod.Get,
                    ApiType.POST => HttpMethod.Post,
                    ApiType.PUT => HttpMethod.Put,
                    ApiType.DELETE => HttpMethod.Delete,
                    ApiType.PATCH => HttpMethod.Patch,
                    _ => HttpMethod.Get
                };

                // Add content if data exists
                if (requestDto.Data != null)
                {
                    message.Content = new StringContent(
                        JsonConvert.SerializeObject(requestDto.Data),
                        Encoding.UTF8,
                        "application/json");
                }
                Console.WriteLine($"Sending message to: {message.RequestUri} with METHOD: {message.Method}");
                // Send request and get response
                HttpResponseMessage apiResponse = await client.SendAsync(message);

                if (!apiResponse.IsSuccessStatusCode)
                {
                    var errorContent = await apiResponse.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error: {apiResponse.StatusCode}, Details: {errorContent}");
                }

                // Read content from response
                var apiContent = await apiResponse.Content.ReadAsStringAsync();
                var apiResponseDto = new ResponseDto();

                // Handle response based on status code
                switch (apiResponse.StatusCode)
                {
                    case HttpStatusCode.OK:
                    case HttpStatusCode.Created:
                    case HttpStatusCode.NoContent:
                        apiResponseDto.IsSuccess = true;
                        apiResponseDto.Result = JsonConvert.DeserializeObject(apiContent);
                        break;
                    case HttpStatusCode.BadRequest:
                        apiResponseDto.IsSuccess = false;
                        apiResponseDto.Message = "Bad Request";
                        apiResponseDto.ErrorMessages = JsonConvert.DeserializeObject<List<string>>(apiContent);
                        break;
                    case HttpStatusCode.NotFound:
                        apiResponseDto.IsSuccess = false;
                        apiResponseDto.Message = "Resource Not Found";
                        break;
                    case HttpStatusCode.Unauthorized:
                        apiResponseDto.IsSuccess = false;
                        apiResponseDto.Message = "Unauthorized";
                        break;
                    case HttpStatusCode.Forbidden:
                        apiResponseDto.IsSuccess = false;
                        apiResponseDto.Message = "Access Denied";
                        break;
                    case HttpStatusCode.InternalServerError:
                        apiResponseDto.IsSuccess = false;
                        apiResponseDto.Message = "Internal Server Error";
                        break;
                    default:
                        apiResponseDto.IsSuccess = false;
                        apiResponseDto.Message = "An error occurred";
                        break;
                }

                return apiResponseDto;
            }
            catch (Exception ex)
            {
                return new ResponseDto
                {
                    IsSuccess = false,
                    Message = ex.Message,
                    ErrorMessages = new List<string> { ex.ToString() }
                };
            }
        }
    }
}
