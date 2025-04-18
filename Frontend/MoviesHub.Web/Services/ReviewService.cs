using MoviesHub.Web.Models;
using MoviesHub.Web.Services.IServices;
using static MoviesHub.Web.Utility.SD;

namespace MoviesHub.Web.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IBaseService _baseService;

        public ReviewService(IBaseService baseService)
        {
            _baseService = baseService;
        }

        public async Task<ResponseDto> GetAllReviewsAsync()
        {
            return await _baseService.SendAsync(new RequestDto
            {
                ApiType = ApiType.GET,
                Url = $"{ReviewAPIBase}/api/reviews"
            });
        }

        public async Task<ResponseDto> GetReviewByIdAsync(int id)
        {
            return await _baseService.SendAsync(new RequestDto
            {
                ApiType = ApiType.GET,
                Url = $"{ReviewAPIBase}/api/reviews/{id}"
            });
        }

        public async Task<ResponseDto> GetReviewsByMovieAsync(int movieId)
        {
            return await _baseService.SendAsync(new RequestDto
            {
                ApiType = ApiType.GET,
                Url = $"{ReviewAPIBase}/api/reviews/movie/{movieId}"
            });
        }

        public async Task<ResponseDto> GetReviewsByUserAsync(string userId)
        {
            return await _baseService.SendAsync(new RequestDto
            {
                ApiType = ApiType.GET,
                Url = $"{ReviewAPIBase}/api/reviews/user/{userId}"
            });
        }

        public async Task<ResponseDto> CreateReviewAsync(ReviewCreateDto review)
        {
            return await _baseService.SendAsync(new RequestDto
            {
                ApiType = ApiType.POST,
                Data = review,
                Url = $"{ReviewAPIBase}/api/reviews"
            });
        }

        public async Task<ResponseDto> UpdateReviewAsync(int id, ReviewUpdateDto review)
        {
            return await _baseService.SendAsync(new RequestDto
            {
                ApiType = ApiType.PUT,
                Data = review,
                Url = $"{ReviewAPIBase}/api/reviews/{id}"
            });
        }

        public async Task<ResponseDto> DeleteReviewAsync(int id)
        {
            return await _baseService.SendAsync(new RequestDto
            {
                ApiType = ApiType.DELETE,
                Url = $"{ReviewAPIBase}/api/reviews/{id}"
            });
        }

        public async Task<ResponseDto> RestoreReviewAsync(int id)
        {
            return await _baseService.SendAsync(new RequestDto
            {
                ApiType = ApiType.PATCH,
                Url = $"{ReviewAPIBase}/api/reviews/{id}/restore"
            });
        }

        public async Task<ResponseDto> GetDeletedReviewsAsync()
        {
            return await _baseService.SendAsync(new RequestDto
            {
                ApiType = ApiType.GET,
                Url = $"{ReviewAPIBase}/api/reviews/deleted"
            });
        }
    }
}
