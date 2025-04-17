using MoviesHub.Services.AuthAPI.Models.Dto;

namespace MoviesHub.Services.AuthAPI.Services.IServices
{
    public interface IAuthService
    {
        Task<string> Register(RegistrationRequestDto registrationRequestDto);
        Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto);
        Task<bool> AssignRole(string email, string roleName);

        Task<bool> AssignRoleWithDto(RoleAssignmentDto roleAssignmentDto);

        Task<List<UserWithRoleDto>> GetUsersWithRoles(); // Nuevo método


    }
}
