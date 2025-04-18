namespace MoviesHub.Services.AuthAPI.Services.IServices
{
    public interface IMovieAPIService
    {
        Task<bool> MovieExistsAsync(int movieId);
        Task<T> GetMovieDetailsAsync<T>(int movieId);
        // Additional methods as needed
    }

    
}
