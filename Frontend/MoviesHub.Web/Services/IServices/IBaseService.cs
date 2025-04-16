using MoviesHub.Web.Models;

namespace MoviesHub.Web.Service.IServices
{
    public interface IBaseService
    {
        Task<ResponseDto> SendAsync(RequestDto requestDto, bool withBearerToken = true);
    }
}
