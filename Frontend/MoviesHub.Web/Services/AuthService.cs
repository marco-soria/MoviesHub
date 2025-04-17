using MoviesHub.Web.Models;
using MoviesHub.Web.Service.IServices;
using MoviesHub.Web.Services.IServices;
using MoviesHub.Web.Utility;

namespace MoviesHub.Web.Services
{
    public class AuthService : IAuthService
    {
        private readonly IBaseService _baseService;

        public AuthService(IBaseService baseService)
        {
            _baseService = baseService;
        }

        public async Task<ResponseDto?> AssignRoleAsync(RegistrationRequestDto registrationRequestDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = registrationRequestDto,
                Url = SD.AuthAPIBase + "/api/auth/assignrole"
            });
        }

        public async Task<ResponseDto?> LoginAsync(LoginRequestDto loginRequestDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = loginRequestDto,
                Url = SD.AuthAPIBase + "/api/auth/login"
            }, withBearerToken: false);
        }

        public async Task<ResponseDto?> RegisterAsync(RegistrationRequestDto registrationRequestDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = registrationRequestDto,
                Url = SD.AuthAPIBase + "/api/auth/register"
            }, withBearerToken: false);
        }

        public async Task<ResponseDto?> AssignRoleWithDtoAsync(RoleAssignmentDto roleAssignmentDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = roleAssignmentDto,
                Url = SD.AuthAPIBase + "/api/auth/assignrolewithDto"
            });
        }

        public async Task<ResponseDto?> GetUsersWithRolesAsync()
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.GET,
                Url = SD.AuthAPIBase + "/api/auth/users-with-roles"
            });
        }


    }
}
