namespace MoviesHub.Services.ReviewsAPI.Services.IServices
{
   
    public interface IMovieAPIService
    {
        Task<bool> MovieExistsAsync(int movieId);
        Task<T> GetMovieDetailsAsync<T>(int movieId);
        // Additional methods as needed
        Task<bool> NotifyRatingChangeAsync(int movieId);

    }
}
