using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace MoviesHub.GatewaySolution.Extensions
{
    public static class WebApplicationBuilderExtensions
    {
        public static WebApplicationBuilder AddAppAuthentication(this WebApplicationBuilder builder)
        {
            var secret = builder.Configuration.GetValue<string>("ApiSettings:Secret");
            var issuer = builder.Configuration.GetValue<string>("ApiSettings:Issuer");
            var audience = builder.Configuration.GetValue<string>("ApiSettings:Audience");

            var Key = Encoding.ASCII.GetBytes(secret);

            builder.Services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer("Bearer", x =>
            {
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Key),
                    ValidateIssuer = true,
                    ValidIssuer = issuer,
                    ValidAudience = audience,
                    ValidateAudience = true,
                };

                // Establecer SaveToken = true para tener el token disponible
                x.SaveToken = true;

                // Añadir logging para eventos de autenticación
                x.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        // Intentar extraer token del encabezado Authorization
                        var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
                        var logger = context.HttpContext.RequestServices
                            .GetRequiredService<ILogger<JwtBearerEvents>>();

                        logger.LogInformation("Auth Header: {AuthHeader}", authHeader ?? "null");

                        if (authHeader != null)
                        {
                            // Si el header no comienza con "Bearer ", tomarlo como token directamente
                            if (!authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                            {
                                context.Token = authHeader;
                                logger.LogInformation("Using raw Authorization header as token");
                            }
                            else
                            {
                                // Extraer token después de "Bearer "
                                context.Token = authHeader.Substring(7);
                                logger.LogInformation("Using Bearer token");
                            }
                        }

                        logger.LogInformation("Token extracted: {Token}",
                            context.Token?.Length > 10
                                ? context.Token.Substring(0, 10) + "..."
                                : context.Token ?? "null");

                        return Task.CompletedTask;
                    },
                    OnTokenValidated = context => {
                        var logger = context.HttpContext.RequestServices
                            .GetRequiredService<ILogger<JwtBearerEvents>>();
                        logger.LogInformation("Token validated successfully for user: {NameID}",
                            context.Principal?.Identity?.Name ?? "unknown");
                        return Task.CompletedTask;
                    },
                    OnAuthenticationFailed = context => {
                        var logger = context.HttpContext.RequestServices
                            .GetRequiredService<ILogger<JwtBearerEvents>>();
                        logger.LogError(context.Exception, "Authentication failed: {Error}",
                            context.Exception.Message);
                        return Task.CompletedTask;
                    },
                    OnChallenge = context => {
                        var logger = context.HttpContext.RequestServices
                            .GetRequiredService<ILogger<JwtBearerEvents>>();
                        logger.LogWarning("Challenge issued for request to {Path}",
                            context.Request.Path);
                        return Task.CompletedTask;
                    }
                };
            });

            return builder;
        }
    }
}
