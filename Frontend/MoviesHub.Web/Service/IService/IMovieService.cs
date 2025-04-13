using MoviesHub.Web.Models;

namespace MoviesHub.Web.Service.IService
{
    public interface IMovieService
    {
        Task<ResponseDto> GetAllMoviesAsync();
        Task<ResponseDto> GetMovieByIdAsync(int id);
        Task<ResponseDto> CreateMovieAsync(MovieCreateDto movie);
        Task<ResponseDto> UpdateMovieAsync(int id, MovieUpdateDto movie);
        Task<ResponseDto> DeleteMovieAsync(int id);
        Task<ResponseDto> GetMoviesByGenreAsync(int genreId);
        Task<ResponseDto> RestoreMovieAsync(int id);
    }
}
