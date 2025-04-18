namespace MoviesHub.Services.AuthAPI.Services.IServices
{
    public interface IReviewAPIService
    {
        Task<T> GetUserReviewsAsync<T>(string userId);
        Task<T> GetMovieReviewStatsAsync<T>(int movieId);

    }
}
