namespace MoviesHub.Services.MoviesAPI.Services.IServices
{
    public interface IReviewAPIService
    {
        Task<T> GetMovieRatingsAsync<T>(int movieId);
        Task<double> GetAverageRatingAsync(int movieId);
        // Additional methods as needed
    }
}
