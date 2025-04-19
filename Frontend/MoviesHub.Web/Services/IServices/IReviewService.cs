using MoviesHub.Web.Models;

namespace MoviesHub.Web.Services.IServices
{
    public interface IReviewService
    {
        Task<ResponseDto> GetAllReviewsAsync();
        Task<ResponseDto> GetReviewByIdAsync(int id);
        Task<ResponseDto> CreateReviewAsync(ReviewCreateDto review);
        Task<ResponseDto> UpdateReviewAsync(int id, ReviewUpdateDto review);
        Task<ResponseDto> DeleteReviewAsync(int id);

        // Métodos para obtener y filtrar reseñas
        Task<ResponseDto> GetReviewsByMovieAsync(int movieId);
        Task<T> GetReviewsByMovieAsync<T>(int movieId);
        Task<ResponseDto> GetReviewsByUserAsync(string userId);
        Task<ResponseDto> GetUserReviewForMovieAsync(int movieId, string userId);

        // Métodos para soft delete
        Task<ResponseDto> RestoreReviewAsync(int id);
        Task<ResponseDto> GetDeletedReviewsAsync();
    }
}
