using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.OpenApi.Models;
using MoviesHub.Services.MoviesAPI;
using MoviesHub.Services.MoviesAPI.Data;
using MoviesHub.Services.MoviesAPI.Extensions;
using MoviesHub.Services.MoviesAPI.Services;
using MoviesHub.Services.MoviesAPI.Services.IServices;
using MoviesHub.Services.MoviesAPI.Utility;
using System.Net;
using Polly;
using Polly.Extensions.Http;

var builder = WebApplication.CreateBuilder(args);

// 1. Servicios fundamentales
builder.Services.AddControllers();

// Agregar caché en memoria
builder.Services.AddMemoryCache();

builder.Services.AddSwaggerGen(option =>
{
    option.AddSecurityDefinition("JWT", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Enter your JWT token directly (without Bearer prefix)",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "JWT"
    });

    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "JWT"
                }
            }, new string[] {}
        }
    });
});

// 2. Configuración de base de datos
builder.Services.AddDbContext<MovieDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 3. Configuración de AutoMapper
IMapper mapper = MappingConfig.RegisterMaps().CreateMapper();
builder.Services.AddSingleton(mapper);
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// 4. Definición de políticas de resiliencia
static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .OrResult(msg => msg.StatusCode == HttpStatusCode.NotFound)
        .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
}

static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .CircuitBreakerAsync(5, TimeSpan.FromSeconds(30));
}

// 5. Configuración de autenticación para llamadas HTTP
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<BackendApiAuthenticationHttpClientHandler>();

// 6. Configuración de HttpClient con políticas
builder.Services.AddHttpClient("AuthAPI", x => x.BaseAddress =
    new Uri(builder.Configuration["ServiceUrls:AuthAPI"]))
    .AddHttpMessageHandler<BackendApiAuthenticationHttpClientHandler>()
    .AddPolicyHandler(GetRetryPolicy())
    .AddPolicyHandler(GetCircuitBreakerPolicy());

builder.Services.AddHttpClient("ReviewsAPI", x => x.BaseAddress =
    new Uri(builder.Configuration["ServiceUrls:ReviewsAPI"]))
    .AddHttpMessageHandler<BackendApiAuthenticationHttpClientHandler>()
    .AddPolicyHandler(GetRetryPolicy())
    .AddPolicyHandler(GetCircuitBreakerPolicy());

// 7. Servicios de API
builder.Services.AddScoped<IAuthAPIService, AuthAPIService>();

// Registrar el servicio ReviewAPIService como implementación concreta
builder.Services.AddScoped<ReviewAPIService>();

// Registrar el servicio cacheado como implementación de la interfaz
builder.Services.AddScoped<IReviewAPIService>(provider => {
    var originalService = provider.GetRequiredService<ReviewAPIService>();
    var memoryCache = provider.GetRequiredService<IMemoryCache>();
    var logger = provider.GetRequiredService<ILogger<CachedReviewAPIService>>();
    return new CachedReviewAPIService(originalService, memoryCache, logger);
});

// 8. Configuración de autenticación y autorización
builder.AddAppAuthentication();
builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
