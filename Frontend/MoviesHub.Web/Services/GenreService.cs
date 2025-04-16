using MoviesHub.Web.Models;
using MoviesHub.Web.Service.IServices;
using static MoviesHub.Web.Utility.SD;

namespace MoviesHub.Web.Services
{
    public class GenreService : IGenreService
    {
        private readonly IBaseService _baseService;

        public GenreService(IBaseService baseService)
        {
            _baseService = baseService;
        }

        public async Task<ResponseDto> GetAllGenresAsync()
        {
            return await _baseService.SendAsync(new RequestDto
            {
                ApiType = ApiType.GET,
                Url = $"{MovieAPIBase}/api/genres"
            });
        }

        public async Task<ResponseDto> GetGenreByIdAsync(int id)
        {
            return await _baseService.SendAsync(new RequestDto
            {
                ApiType = ApiType.GET,
                Url = $"{MovieAPIBase}/api/genres/{id}"
            });
        }

        public async Task<ResponseDto> CreateGenreAsync(GenreCreateDto genre)
        {
            return await _baseService.SendAsync(new RequestDto
            {
                ApiType = ApiType.POST,
                Data = genre,
                Url = $"{MovieAPIBase}/api/genres"
            });
        }

        public async Task<ResponseDto> UpdateGenreAsync(int id, GenreUpdateDto genre)
        {
            return await _baseService.SendAsync(new RequestDto
            {
                ApiType = ApiType.PUT,
                Data = genre,
                Url = $"{MovieAPIBase}/api/genres/{id}"
            });
        }

        // ...existing code...
        public async Task<ResponseDto> DeleteGenreAsync(int id)
        {
            // Ensure the URL includes the ID parameter
            return await _baseService.SendAsync(new RequestDto
            {
                ApiType = ApiType.DELETE,
                Url = $"{MovieAPIBase}/api/genres/{id}" // Correctly format the URL
            });
        }

        public async Task<ResponseDto> RestoreGenreAsync(int id)
        {
            return await _baseService.SendAsync(new RequestDto
            {
                ApiType = ApiType.PATCH,
                Url = $"{MovieAPIBase}/api/genres/{id}/restore"
            });
        }

        public async Task<ResponseDto> GetMoviesForGenreAsync(int id)
        {
            return await _baseService.SendAsync(new RequestDto
            {
                ApiType = ApiType.GET,
                Url = $"{MovieAPIBase}/api/genres/{id}/movies"
            });
        }

        public async Task<ResponseDto> GetDeletedGenresAsync()
        {
            return await _baseService.SendAsync(new RequestDto
            {
                ApiType = ApiType.GET,
                Url = $"{MovieAPIBase}/api/genres/deleted"
            });
        }
    }
}
