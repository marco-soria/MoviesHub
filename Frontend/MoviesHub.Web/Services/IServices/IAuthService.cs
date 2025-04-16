using MoviesHub.Web.Models;

namespace MoviesHub.Web.Services.IServices
{
    public interface IAuthService
    {
        Task<ResponseDto?> RegisterAsync(RegistrationRequestDto registrationRequestDto);

        Task<ResponseDto?> LoginAsync(LoginRequestDto loginRequestDto);

        Task<ResponseDto?> AssignRoleAsync(RegistrationRequestDto registrationRequestDto);
    }
}
