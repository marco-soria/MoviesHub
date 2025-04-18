using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using MoviesHub.Services.ReviewsAPI;
using MoviesHub.Services.ReviewsAPI.Data;
using MoviesHub.Services.ReviewsAPI.Extensions;
using MoviesHub.Services.ReviewsAPI.Services;
using MoviesHub.Services.ReviewsAPI.Services.IServices;
using MoviesHub.Services.ReviewsAPI.Utility;
using System.Net;
using Polly;
using Polly.Extensions.Http;

var builder = WebApplication.CreateBuilder(args);

// 1. Servicios fundamentales
builder.Services.AddControllers();

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
builder.Services.AddDbContext<ReviewDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

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
builder.Services.AddHttpClient("Movies", x => x.BaseAddress =
    new Uri(builder.Configuration["ServiceUrls:MoviesAPI"]))
    .AddHttpMessageHandler<BackendApiAuthenticationHttpClientHandler>()
    .AddPolicyHandler(GetRetryPolicy())
    .AddPolicyHandler(GetCircuitBreakerPolicy());

builder.Services.AddHttpClient("Auth", x => x.BaseAddress =
    new Uri(builder.Configuration["ServiceUrls:AuthAPI"]))
    .AddHttpMessageHandler<BackendApiAuthenticationHttpClientHandler>()
    .AddPolicyHandler(GetRetryPolicy())
    .AddPolicyHandler(GetCircuitBreakerPolicy());

// 7. Servicios de API
builder.Services.AddScoped<IAuthAPIService, AuthAPIService>();
builder.Services.AddScoped<IMovieAPIService, MovieAPIService>();

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

ApplyMigration();

app.Run();

void ApplyMigration()
{
    using (var scope = app.Services.CreateScope())
    {
        var _db = scope.ServiceProvider.GetRequiredService<ReviewDbContext>();
        if (_db.Database.GetPendingMigrations().Count() > 0)
        {
            _db.Database.Migrate();
        }
    }
}
