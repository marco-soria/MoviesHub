using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MoviesHub.Services.AuthAPI.Data;
using MoviesHub.Services.AuthAPI.Models;
using MoviesHub.Services.AuthAPI.Services.IServices;
using MoviesHub.Services.AuthAPI.Services;
using MoviesHub.Services.AuthAPI.Utility;
using System.Net;
using Polly;
using Polly.Extensions.Http;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// 1. Servicios fundamentales
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

// 2. Configuración de base de datos y autenticación
builder.Services.AddDbContext<AuthDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

//builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("ApiSettings:JwtOptions"));
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("ApiSettings"));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<AuthDbContext>()
    .AddDefaultTokenProviders();

// Añade esto después de la configuración de Identity
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options => {
    var jwtOptions = builder.Configuration.GetSection("ApiSettings").Get<JwtOptions>();

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Secret)),
            ValidateIssuer = true,
            ValidIssuer = jwtOptions.Issuer,
            ValidateAudience = true,
            ValidAudience = jwtOptions.Audience,
            // Añade estas opciones para ser más flexible con los claims
            NameClaimType = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name",
            RoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"
        };
//.AddJwtBearer(options =>
//{
//    options.TokenValidationParameters = new TokenValidationParameters
//    {
//        ValidateIssuerSigningKey = true,
//        IssuerSigningKey = new SymmetricSecurityKey(
//            Encoding.ASCII.GetBytes(builder.Configuration.GetValue<string>("ApiSettings:Secret"))),
//        ValidateIssuer = true,
//        ValidIssuer = builder.Configuration.GetValue<string>("ApiSettings:Issuer"),
//        ValidateAudience = true,
//        ValidAudience = builder.Configuration.GetValue<string>("ApiSettings:Audience")
//    };

    options.Events = new JwtBearerEvents
    {
        OnTokenValidated = context =>
        {
            Console.WriteLine($"==== TOKEN VALIDADO CORRECTAMENTE ====");
            Console.WriteLine($"Usuario: {context.Principal?.Identity?.Name ?? "null"}");
            Console.WriteLine($"====================================");
            return Task.CompletedTask;
        },
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine($"==== ERROR DE AUTENTICACIÓN JWT ====");
            Console.WriteLine($"Error: {context.Exception.Message}");
            Console.WriteLine($"StackTrace: {context.Exception.StackTrace}");
            Console.WriteLine($"====================================");
            return Task.CompletedTask;
        },
        OnChallenge = context =>
        {
            Console.WriteLine($"==== CHALLENGE DE AUTENTICACIÓN ====");
            Console.WriteLine($"URL: {context.Request.Path}");
            Console.WriteLine($"====================================");
            return Task.CompletedTask;
        },
        OnMessageReceived = context =>
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault();
            Console.WriteLine($"==== TOKEN RECIBIDO ====");
            Console.WriteLine($"Token: {token?.Substring(0, Math.Min(20, token?.Length ?? 0)) ?? "null"}...");
            Console.WriteLine($"====================================");
            return Task.CompletedTask;
        }
    };
});


// 3. Servicios propios
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<DataInitializer>();

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
builder.Services.AddHttpClient("MoviesAPI", x => x.BaseAddress =
    new Uri(builder.Configuration["ServiceUrls:MoviesAPI"]))
    .AddHttpMessageHandler<BackendApiAuthenticationHttpClientHandler>()
    .AddPolicyHandler(GetRetryPolicy())
    .AddPolicyHandler(GetCircuitBreakerPolicy());

builder.Services.AddHttpClient("ReviewsAPI", x => x.BaseAddress =
    new Uri(builder.Configuration["ServiceUrls:ReviewsAPI"]))
    .AddHttpMessageHandler<BackendApiAuthenticationHttpClientHandler>()
    .AddPolicyHandler(GetRetryPolicy())
    .AddPolicyHandler(GetCircuitBreakerPolicy());

// 7. Servicios de API
builder.Services.AddScoped<IMovieAPIService, MovieAPIService>();
builder.Services.AddScoped<IReviewAPIService, ReviewAPIService>();

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

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var initializer = services.GetRequiredService<DataInitializer>();
        await initializer.InitializeAsync();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Error al inicializar la base de datos.");
    }
}

ApplyMigration();

app.Run();

void ApplyMigration()
{
    using (var scope = app.Services.CreateScope())
    {
        var _db = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
        if (_db.Database.GetPendingMigrations().Count() > 0)
        {
            _db.Database.Migrate();
        }
    }
}
