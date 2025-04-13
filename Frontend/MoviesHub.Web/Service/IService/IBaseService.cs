using MoviesHub.Web.Models;

namespace MoviesHub.Web.Service.IService
{
    public interface IBaseService
    {
        Task<ResponseDto> SendAsync(RequestDto requestDto, bool withBearerToken = true);
    }
}
