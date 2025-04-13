using MoviesHub.Web.Models;
using MoviesHub.Web.Service.IService;
using static MoviesHub.Web.Utility.SD;

namespace MoviesHub.Web.Service
{
    public class MovieService : IMovieService
    {
        private readonly IBaseService _baseService;

        public MovieService(IBaseService baseService)
        {
            _baseService = baseService;
        }

        public async Task<ResponseDto> GetAllMoviesAsync()
        {
            return await _baseService.SendAsync(new RequestDto
            {
                ApiType = ApiType.GET,
                Url = $"{MovieAPIBase}/api/movies"
            });
        }

        public async Task<ResponseDto> GetMovieByIdAsync(int id)
        {
            return await _baseService.SendAsync(new RequestDto
            {
                ApiType = ApiType.GET,
                Url = $"{MovieAPIBase}/api/movies/{id}"
            });
        }

        public async Task<ResponseDto> CreateMovieAsync(MovieCreateDto movie)
        {
            return await _baseService.SendAsync(new RequestDto
            {
                ApiType = ApiType.POST,
                Data = movie,
                Url = $"{MovieAPIBase}/api/movies"
            });
        }

        public async Task<ResponseDto> UpdateMovieAsync(int id, MovieUpdateDto movie)
        {
            return await _baseService.SendAsync(new RequestDto
            {
                ApiType = ApiType.PUT,
                Data = movie,
                Url = $"{MovieAPIBase}/api/movies/{id}"
            });
        }

        public async Task<ResponseDto> DeleteMovieAsync(int id)
        {
            return await _baseService.SendAsync(new RequestDto
            {
                ApiType = ApiType.DELETE,
                Url = $"{MovieAPIBase}/api/movies/{id}"
            });
        }

        public async Task<ResponseDto> RestoreMovieAsync(int id)
        {
            return await _baseService.SendAsync(new RequestDto
            {
                ApiType = ApiType.PATCH,
                Url = $"{MovieAPIBase}/api/movies/{id}/restore"
            });
        }

        public async Task<ResponseDto> GetMoviesByGenreAsync(int genreId)
        {
            return await _baseService.SendAsync(new RequestDto
            {
                ApiType = ApiType.GET,
                Url = $"{MovieAPIBase}/api/movies/bygenre/{genreId}"
            });
        }
    }
}
