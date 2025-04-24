using Microsoft.Extensions.Logging;
using MoviesHub.GatewaySolution.Extensions;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

// 1. Servicios esenciales
builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers(); // Para endpoints básicos como /health

// 2. Configuración de logging
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.SetMinimumLevel(LogLevel.Information);

// 3. Autenticación y handlers
builder.AddAppAuthentication(); // Configuración JWT
builder.Services.AddTransient<BackendApiAuthenticationDelegatingHandler>(); // Para propagación de tokens

// 4. Configuración de Ocelot
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);
builder.Services.AddOcelot(builder.Configuration);

// 5. CORS (lo mantenemos ya que podría ser necesario para APIs)
builder.Services.AddCors(options => {
    options.AddPolicy("AllowAll",
        corsBuilder => corsBuilder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

var app = builder.Build();

// 1. Middleware global
if (app.Environment.IsDevelopment())
{
    // Middleware de logging solo en desarrollo
    app.Use(async (context, next) =>
    {
        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
        logger.LogInformation("Request: {Method} {Path}", context.Request.Method, context.Request.Path);

        try
        {
            await next();
            logger.LogInformation("Response: {StatusCode} for {Path}",
                context.Response.StatusCode, context.Request.Path);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error processing {Method} {Path}",
                context.Request.Method, context.Request.Path);
            throw;
        }
    });
}

// 2. Middlewares estándar
app.UseRouting();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();

// 3. Endpoints básicos
app.MapWhen(
    ctx => ctx.Request.Path.Value == "/" || ctx.Request.Path.Value == "/health",
    appBuilder => {
        appBuilder.Run(async context => {
            if (context.Request.Path.Value == "/")
            {
                await context.Response.WriteAsync("MoviesHub API Gateway: Hello World");
            }
            else if (context.Request.Path.Value == "/health")
            {
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(
                    new { Status = "Healthy", Timestamp = DateTime.UtcNow }
                ));
            }
        });
    }
);

// 4. Configuración de Ocelot
await app.UseOcelot();

app.Run();



