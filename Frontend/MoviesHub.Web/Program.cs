
using MoviesHub.Web.Utility;
using Microsoft.AspNetCore.Authentication.Cookies;
using MoviesHub.Web.Service.IServices;
using MoviesHub.Web.Services;
using MoviesHub.Web.Services.IServices;

var builder = WebApplication.CreateBuilder(args);

// Configure API base URLs from appsettings.json
SD.MovieAPIBase = builder.Configuration["ServiceUrls:MovieAPI"]!;
SD.ReviewAPIBase = builder.Configuration["ServiceUrls:ReviewAPI"]!;
SD.AuthAPIBase = builder.Configuration["ServiceUrls:AuthAPI"]!;

// Register services
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();
builder.Services.AddHttpClient<IGenreService, GenreService>();
builder.Services.AddHttpClient<IMovieService, MovieService>();
builder.Services.AddHttpClient<IBaseService, BaseService>();
builder.Services.AddHttpClient<IMovieGenreService, MovieGenreService>();
builder.Services.AddHttpClient<IAuthService, AuthService>();




builder.Services.AddScoped<ITokenProvider, TokenProvider>();
builder.Services.AddScoped<IBaseService, BaseService>();
builder.Services.AddScoped<IMovieService, MovieService>();
builder.Services.AddScoped<IGenreService, GenreService>();
builder.Services.AddScoped<IMovieGenreService, MovieGenreService>();
builder.Services.AddScoped<IAuthService, AuthService>();

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.ExpireTimeSpan = TimeSpan.FromHours(10);
        options.LoginPath = "/Auth/Login";
        options.AccessDeniedPath = "/Auth/AccessDenied";
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
