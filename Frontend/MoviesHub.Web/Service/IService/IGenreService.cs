using MoviesHub.Web.Models;

namespace MoviesHub.Web.Service.IService
{
    public interface IGenreService
    {
        Task<ResponseDto> GetAllGenresAsync();
        Task<ResponseDto> GetGenreByIdAsync(int id);
        Task<ResponseDto> CreateGenreAsync(GenreCreateDto genre);
        Task<ResponseDto> UpdateGenreAsync(int id, GenreUpdateDto genre);
        Task<ResponseDto> DeleteGenreAsync(int id);
        Task<ResponseDto> RestoreGenreAsync(int id);
        Task<ResponseDto> GetMoviesForGenreAsync(int id);

        Task<ResponseDto> GetDeletedGenresAsync();
    }
}
