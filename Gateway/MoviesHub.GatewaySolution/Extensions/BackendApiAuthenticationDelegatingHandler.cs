namespace MoviesHub.GatewaySolution.Extensions
{
    public class BackendApiAuthenticationDelegatingHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<BackendApiAuthenticationDelegatingHandler> _logger;

        public BackendApiAuthenticationDelegatingHandler(
            IHttpContextAccessor httpContextAccessor,
            ILogger<BackendApiAuthenticationDelegatingHandler> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var authHeader = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].FirstOrDefault();

            if (!string.IsNullOrEmpty(authHeader))
            {
                // Asegúrate de que el encabezado se envía correctamente
                request.Headers.Remove("Authorization");

                // Usa el método correcto para agregar el encabezado
                if (!request.Headers.TryAddWithoutValidation("Authorization", authHeader))
                {
                    _logger.LogWarning("No se pudo agregar el encabezado Authorization a la solicitud downstream");
                }
                else
                {
                    _logger.LogInformation("Token propagado correctamente a solicitud downstream: {RequestUri}", request.RequestUri);
                }
            }
            else
            {
                _logger.LogWarning("No se encontró token de autorización para propagar a: {RequestUri}", request.RequestUri);
            }

            try
            {
                var response = await base.SendAsync(request, cancellationToken);

                // Log de respuesta para diagnóstico
                _logger.LogInformation("Respuesta de {Uri}: StatusCode={StatusCode}",
                    request.RequestUri, response.StatusCode);

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error enviando solicitud a {Uri}", request.RequestUri);
                throw;
            }
        }
    }
}
