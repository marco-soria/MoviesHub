using MoviesHub.Services.AuthAPI.Models.Dto;

namespace MoviesHub.Services.AuthAPI.Services.IServices
{
    public interface IUserService
    {
        Task<List<UserResponseDto>> GetAllUsersAsync(bool includeDeleted = false);
        Task<UserResponseDto?> GetUserByIdAsync(string id);
        Task<UserResponseDto?> GetUserByEmailAsync(string email);
        Task<UserResponseDto> CreateUserAsync(UserRequestDto userRequestDto);
        Task<UserResponseDto?> UpdateUserAsync(string id, UserRequestDto userRequestDto);
        Task<bool> DeleteUserAsync(string id, bool permanent = false);
        Task<bool> RestoreUserAsync(string id);
    }
}
