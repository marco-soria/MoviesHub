using System.Net.Http.Headers;

namespace MoviesHub.Services.MoviesAPI.Extensions
{
    
    public class CustomJwtBearerHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request.Headers.Authorization != null &&
                request.Headers.Authorization.Scheme == "Bearer" &&
                !request.Headers.Authorization.Parameter.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                string originalToken = request.Headers.Authorization.Parameter;
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", originalToken);
            }
            return await base.SendAsync(request, cancellationToken);
        }
    }

}
