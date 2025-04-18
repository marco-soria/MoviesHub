using Microsoft.AspNetCore.Authentication;
using System.Net.Http.Headers;

namespace MoviesHub.Services.AuthAPI.Utility
{
    public class BackendApiAuthenticationHttpClientHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _accessor;

        public BackendApiAuthenticationHttpClientHandler(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var token = await _accessor.HttpContext.GetTokenAsync("access_token");

            // Set the Authorization header with the Bearer token
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Handle case where token might already include "Bearer" prefix
            if (request.Headers.Authorization != null &&
                request.Headers.Authorization.Scheme == "Bearer" &&
                request.Headers.Authorization.Parameter?.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase) == true)
            {
                string originalToken = request.Headers.Authorization.Parameter;
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", originalToken.Substring(7).Trim());
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
