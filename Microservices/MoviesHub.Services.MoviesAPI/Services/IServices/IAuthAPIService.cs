namespace MoviesHub.Services.MoviesAPI.Services.IServices
{
    public interface IAuthAPIService
    {
        Task<bool> ValidateTokenAsync(string token);
        Task<T> GetUserDetailsAsync<T>(string userId);
        // Additional methods as needed
    }

   
}
