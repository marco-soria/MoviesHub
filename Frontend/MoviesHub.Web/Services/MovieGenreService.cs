using MoviesHub.Web.Models;
using MoviesHub.Web.Services.IServices;
using static MoviesHub.Web.Utility.SD;

namespace MoviesHub.Web.Services
{
    public class MovieGenreService : IMovieGenreService
    {
        private readonly IBaseService _baseService;

        public MovieGenreService(IBaseService baseService)
        {
            _baseService = baseService;
        }

        public async Task<ResponseDto> GetAllMovieGenresAsync()
        {
            return await _baseService.SendAsync(new RequestDto
            {
                ApiType = ApiType.GET,
                Url = $"{MovieAPIBase}/api/movie-genres"
            });
        }

        public async Task<ResponseDto> GetMovieGenreAsync(int movieId, int genreId)
        {
            return await _baseService.SendAsync(new RequestDto
            {
                ApiType = ApiType.GET,
                Url = $"{MovieAPIBase}/api/movie-genres/{movieId}/{genreId}"
            });
        }

        public async Task<ResponseDto> CreateMovieGenreAsync(MovieGenreCreateDto movieGenre)
        {
            return await _baseService.SendAsync(new RequestDto
            {
                ApiType = ApiType.POST,
                Data = movieGenre,
                Url = $"{MovieAPIBase}/api/movie-genres"
            });
        }

        public async Task<ResponseDto> DeleteMovieGenreAsync(int movieId, int genreId)
        {
            return await _baseService.SendAsync(new RequestDto
            {
                ApiType = ApiType.DELETE,
                Url = $"{MovieAPIBase}/api/movie-genres/{movieId}/{genreId}"
            });
        }

        public async Task<ResponseDto> GetGenresForMovieAsync(int movieId)
        {
            return await _baseService.SendAsync(new RequestDto
            {
                ApiType = ApiType.GET,
                Url = $"{MovieAPIBase}/api/movie-genres/movie/{movieId}/genres"
            });
        }

        public async Task<ResponseDto> GetMoviesForGenreAsync(int genreId)
        {
            return await _baseService.SendAsync(new RequestDto
            {
                ApiType = ApiType.GET,
                Url = $"{MovieAPIBase}/api/movie-genres/genre/{genreId}/movies"
            });
        }
    }
}
