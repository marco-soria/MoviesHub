using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MoviesHub.Services.AuthAPI.Models;
using MoviesHub.Services.AuthAPI.Data;

namespace MoviesHub.Services.AuthAPI.Data
{
    public class DataInitializer
    {
        private readonly AuthDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<DataInitializer> _logger;

        public DataInitializer(
            AuthDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<DataInitializer> logger)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        public async Task InitializeAsync()
        {
            try
            {
                // Asegurarse de que la base de datos esté creada y aplicar migraciones pendientes
                await _context.Database.MigrateAsync();

                // Verificar si ya hay usuarios en la base de datos
                if (await _context.Users.AnyAsync())
                {
                    _logger.LogInformation("La base de datos ya está inicializada con datos.");
                    return;
                }

                _logger.LogInformation("Iniciando la inicialización de datos...");

                // Crear roles
                await CreateRolesAsync();

                // Crear usuarios
                await CreateUsersAsync();

                _logger.LogInformation("Inicialización de datos completada exitosamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error durante la inicialización de datos.");
                throw;
            }
        }

        private async Task CreateRolesAsync()
        {
            string[] roleNames = { "Admin", "User", "Manager" };

            foreach (var roleName in roleNames)
            {
                if (!await _roleManager.RoleExistsAsync(roleName))
                {
                    await _roleManager.CreateAsync(new IdentityRole(roleName));
                    _logger.LogInformation($"Rol '{roleName}' creado exitosamente.");
                }
            }
        }

        private async Task CreateUsersAsync()
        {
            // Lista de usuarios a crear
            var users = new List<(ApplicationUser User, string Password, string Role)>
            {
                // Admin
                (new ApplicationUser
                {
                    UserName = "admin@example.com",
                    Email = "admin@example.com",
                    FirstName = "Admin",
                    LastName = "Principal",
                    CreatedAt = DateTime.Parse("2023-01-01"),
                    EmailConfirmed = true,
                    IsDeleted = false
                }, "Pass123!", "Admin"),
                
                // Manager
                (new ApplicationUser
                {
                    UserName = "manager@example.com",
                    Email = "manager@example.com",
                    FirstName = "Manager",
                    LastName = "Manager",
                    CreatedAt = DateTime.Parse("2023-01-02"),
                    EmailConfirmed = true,
                    IsDeleted = false
                }, "Pass123!", "Manager"),
                
                // Usuarios normales
                (new ApplicationUser
                {
                    UserName = "john@example.com",
                    Email = "john@example.com",
                    FirstName = "John",
                    LastName = "Doe",
                    CreatedAt = DateTime.Parse("2023-01-03"),
                    EmailConfirmed = true,
                    IsDeleted = false
                }, "Pass123!", "User"),

                (new ApplicationUser
                {
                    UserName = "jane@example.com",
                    Email = "jane@example.com",
                    FirstName = "Jane",
                    LastName = "Doe",
                    CreatedAt = DateTime.Parse("2023-01-04"),
                    EmailConfirmed = true,
                    IsDeleted = false
                }, "Pass123!", "User"),

                (new ApplicationUser
                {
                    UserName = "alice@example.com",
                    Email = "alice@example.com",
                    FirstName = "Alice",
                    LastName = "Johnson",
                    CreatedAt = DateTime.Parse("2023-01-05"),
                    EmailConfirmed = true,
                    IsDeleted = false
                }, "Pass123!", "User"),

                (new ApplicationUser
                {
                    UserName = "bob@example.com",
                    Email = "bob@example.com",
                    FirstName = "Bob",
                    LastName = "Smith",
                    CreatedAt = DateTime.Parse("2023-01-06"),
                    EmailConfirmed = true,
                    IsDeleted = false
                }, "Pass123!", "User"),

                (new ApplicationUser
                {
                    UserName = "emma@example.com",
                    Email = "emma@example.com",
                    FirstName = "Emma",
                    LastName = "Williams",
                    CreatedAt = DateTime.Parse("2023-01-07"),
                    EmailConfirmed = true,
                    IsDeleted = false
                }, "Pass123!", "User"),

                (new ApplicationUser
                {
                    UserName = "michael@example.com",
                    Email = "michael@example.com",
                    FirstName = "Michael",
                    LastName = "Brown",
                    CreatedAt = DateTime.Parse("2023-01-08"),
                    EmailConfirmed = true,
                    IsDeleted = false
                }, "Pass123!", "User"),

                (new ApplicationUser
                {
                    UserName = "sarita@gmail.com",
                    Email = "sarita@gmail.com",
                    FirstName = "sarita",
                    LastName = "sarita",
                    CreatedAt = DateTime.Parse("2023-01-09"),
                    EmailConfirmed = true,
                    IsDeleted = false
                }, "Pass123!", "User"),

                (new ApplicationUser
                {
                    UserName = "david@example.com",
                    Email = "david@example.com",
                    FirstName = "David",
                    LastName = "Miller",
                    CreatedAt = DateTime.Parse("2023-01-10"),
                    EmailConfirmed = true,
                    IsDeleted = false
                }, "Pass123!", "User")
            };

            foreach (var (user, password, role) in users)
            {
                var result = await _userManager.CreateAsync(user, password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, role);
                    _logger.LogInformation($"Usuario '{user.UserName}' creado y asignado al rol '{role}'.");
                }
                else
                {
                    _logger.LogWarning($"Error al crear usuario '{user.UserName}': {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
            }
        }
    }
}