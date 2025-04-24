using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MoviesHub.Services.AuthAPI.Controllers
{
    [Route("api/diagnostic")]
    [ApiController]
    public class DiagnosticController : ControllerBase
    {
        [HttpGet("auth-test")]
        public IActionResult TestAuth()
        {
            var authHeader = Request.Headers["Authorization"].ToString();
            var isAuthenticated = User?.Identity?.IsAuthenticated ?? false;
            var userName = User?.Identity?.Name ?? "null";

            var claims = new Dictionary<string, string>();
            if (User != null)
            {
                foreach (var claim in User.Claims)
                {
                    claims[claim.Type] = claim.Value;
                }
            }

            return Ok(new
            {
                Message = "Auth diagnostics",
                HasAuthHeader = !string.IsNullOrEmpty(authHeader),
                AuthHeader = !string.IsNullOrEmpty(authHeader) ?
                             authHeader.Substring(0, Math.Min(20, authHeader.Length)) + "..." :
                             "null",
                IsAuthenticated = isAuthenticated,
                UserName = userName,
                Claims = claims,
                IsAdmin = User?.IsInRole("Admin") ?? false,
                IsManager = User?.IsInRole("Manager") ?? false
            });
        }
    }
}

