using MoviesHub.Web.Models;

namespace MoviesHub.Web.Services.IServices
{
    public interface IUserService
    {
        Task<ResponseDto?> GetAllUsersAsync(bool includeDeleted = false);
        Task<ResponseDto?> GetUserByIdAsync(string id);
        Task<ResponseDto?> GetUserByEmailAsync(string email);
        Task<ResponseDto?> CreateUserAsync(UserRequestDto userRequestDto);
        Task<ResponseDto?> UpdateUserAsync(string id, UserRequestDto userRequestDto);
        Task<ResponseDto?> DeleteUserAsync(string id, bool permanent = false);
        Task<ResponseDto?> RestoreUserAsync(string id);
    }
}
