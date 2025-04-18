using MoviesHub.Web.Models;

namespace MoviesHub.Web.Services.IServices
{
    public interface IReviewService
    {
        Task<ResponseDto> GetAllReviewsAsync();
        Task<ResponseDto> GetReviewByIdAsync(int id);
        Task<ResponseDto> GetReviewsByMovieAsync(int movieId);
        Task<ResponseDto> GetReviewsByUserAsync(string userId);
        Task<ResponseDto> CreateReviewAsync(ReviewCreateDto review);
        Task<ResponseDto> UpdateReviewAsync(int id, ReviewUpdateDto review);
        Task<ResponseDto> DeleteReviewAsync(int id);
        Task<ResponseDto> RestoreReviewAsync(int id);
        Task<ResponseDto> GetDeletedReviewsAsync();
    }
}
