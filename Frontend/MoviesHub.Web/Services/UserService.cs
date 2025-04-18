using MoviesHub.Web.Models;
using MoviesHub.Web.Services.IServices;
using MoviesHub.Web.Utility;

namespace MoviesHub.Web.Services
{
    public class UserService : IUserService
    {
        private readonly IBaseService _baseService;

        public UserService(IBaseService baseService)
        {
            _baseService = baseService;
        }

        public async Task<ResponseDto?> GetAllUsersAsync(bool includeDeleted = false)
        {
            return await _baseService.SendAsync(new RequestDto
            {
                ApiType = SD.ApiType.GET,
                Url = $"{SD.AuthAPIBase}/api/users?includeDeleted={includeDeleted}"
            });
        }

        public async Task<ResponseDto?> GetUserByIdAsync(string id)
        {
            return await _baseService.SendAsync(new RequestDto
            {
                ApiType = SD.ApiType.GET,
                Url = $"{SD.AuthAPIBase}/api/users/{id}"
            });
        }

        public async Task<ResponseDto?> GetUserByEmailAsync(string email)
        {
            return await _baseService.SendAsync(new RequestDto
            {
                ApiType = SD.ApiType.GET,
                Url = $"{SD.AuthAPIBase}/api/users/email/{email}"
            });
        }

        public async Task<ResponseDto?> CreateUserAsync(UserRequestDto userRequestDto)
        {
            return await _baseService.SendAsync(new RequestDto
            {
                ApiType = SD.ApiType.POST,
                Data = userRequestDto,
                Url = $"{SD.AuthAPIBase}/api/users"
            });
        }

        public async Task<ResponseDto?> UpdateUserAsync(string id, UserRequestDto userRequestDto)
        {
            return await _baseService.SendAsync(new RequestDto
            {
                ApiType = SD.ApiType.PUT,
                Data = userRequestDto,
                Url = $"{SD.AuthAPIBase}/api/users/{id}"
            });
        }

        public async Task<ResponseDto?> DeleteUserAsync(string id, bool permanent = false)
        {
            return await _baseService.SendAsync(new RequestDto
            {
                ApiType = SD.ApiType.DELETE,
                Url = $"{SD.AuthAPIBase}/api/users/{id}?permanent={permanent}"
            });
        }

        public async Task<ResponseDto?> RestoreUserAsync(string id)
        {
            return await _baseService.SendAsync(new RequestDto
            {
                ApiType = SD.ApiType.POST,
                Url = $"{SD.AuthAPIBase}/api/users/{id}/restore"
            });
        }
    }
}
