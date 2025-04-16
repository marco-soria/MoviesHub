using MoviesHub.Web.Models;

namespace MoviesHub.Web.Service.IServices
{
    public interface IMovieGenreService
    {
        Task<ResponseDto> GetAllMovieGenresAsync();
        Task<ResponseDto> GetMovieGenreAsync(int movieId, int genreId);
        Task<ResponseDto> CreateMovieGenreAsync(MovieGenreCreateDto movieGenre);
        Task<ResponseDto> DeleteMovieGenreAsync(int movieId, int genreId);
        Task<ResponseDto> GetGenresForMovieAsync(int movieId);
        Task<ResponseDto> GetMoviesForGenreAsync(int genreId);
    }
}
