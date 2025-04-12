using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MoviesHub.Services.AuthAPI.Models;

namespace MoviesHub.Services.AuthAPI.Data
{
    public class AuthDbContext : IdentityDbContext<ApplicationUser>
    {
        public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Índice para búsquedas por email (adicional al que Identity ya crea)
            builder.Entity<ApplicationUser>()
                .HasIndex(u => u.Email);

            // Índice para búsquedas por nombre
            builder.Entity<ApplicationUser>()
                .HasIndex(u => new { u.FirstName, u.LastName });

            // Soft Delete filter
            builder.Entity<ApplicationUser>().HasQueryFilter(u => !u.IsDeleted);

            // Roles
            builder.Entity<IdentityRole>().HasData(
                new IdentityRole { Id = "1", Name = "User", NormalizedName = "USER" },
                new IdentityRole { Id = "2", Name = "Admin", NormalizedName = "ADMIN" },
                new IdentityRole { Id = "3", Name = "Manager", NormalizedName = "MANAGER" }
            );

            // Users (con hasher simplificado)
            var hasher = new PasswordHasher<ApplicationUser>();
            var users = new ApplicationUser[]
            {
                 // Admin
                new ApplicationUser
                {
                    Id = "1",
                    UserName = "admin",
                    NormalizedUserName = "ADMIN",
                    Email = "admin@example.com",
                    NormalizedEmail = "ADMIN@EXAMPLE.COM",
                    EmailConfirmed = true,
                    PasswordHash = hasher.HashPassword(new ApplicationUser(), "Test123!"), // Fix: Replace null with a new ApplicationUser instance
                    SecurityStamp = Guid.NewGuid().ToString(),
                    FirstName = "Admin",
                    LastName = "Principal",
                    CreatedAt = DateTime.Parse("2023-01-01"),
                    IsDeleted = false
                },
                
                // Manager
                new ApplicationUser
                {
                    Id = "2",
                    UserName = "manager",
                    NormalizedUserName = "MANAGER",
                    Email = "manager@example.com",
                    NormalizedEmail = "MANAGER@EXAMPLE.COM",
                    EmailConfirmed = true,
                    PasswordHash = hasher.HashPassword(new ApplicationUser(), "Test123!"),
                    SecurityStamp = Guid.NewGuid().ToString(),
                    FirstName = "Manager",
                    LastName = "DeCine",
                    CreatedAt = DateTime.Parse("2023-01-02"),
                    IsDeleted = false
                },
                // Usuarios normales
                new ApplicationUser
                {
                    Id = "3",
                    UserName = "johndoe",
                    NormalizedUserName = "JOHNDOE",
                    Email = "john@example.com",
                    NormalizedEmail = "JOHN@EXAMPLE.COM",
                    EmailConfirmed = true,
                    PasswordHash = hasher.HashPassword(new ApplicationUser(), "Test123!"),
                    SecurityStamp = Guid.NewGuid().ToString(),
                    FirstName = "John",
                    LastName = "Doe",
                    CreatedAt = DateTime.Parse("2023-01-03"),
                    IsDeleted = false
                },
                new ApplicationUser
                {
                    Id = "4",
                    UserName = "janedoe",
                    NormalizedUserName = "JANEDOE",
                    Email = "jane@example.com",
                    NormalizedEmail = "JANE@EXAMPLE.COM",
                    EmailConfirmed = true,
                    PasswordHash = hasher.HashPassword(new ApplicationUser(), "Test123!"),
                    SecurityStamp = Guid.NewGuid().ToString(),
                    FirstName = "Jane",
                    LastName = "Doe",
                    CreatedAt = DateTime.Parse("2023-01-04"),
                    IsDeleted = false
                },
                new ApplicationUser
                {
                    Id = "5",
                    UserName = "alice",
                    NormalizedUserName = "ALICE",
                    Email = "alice@example.com",
                    NormalizedEmail = "ALICE@EXAMPLE.COM",
                    EmailConfirmed = true,
                    PasswordHash = hasher.HashPassword(new ApplicationUser(), "Test123!"),
                    SecurityStamp = Guid.NewGuid().ToString(),
                    FirstName = "Alice",
                    LastName = "Johnson",
                    CreatedAt = DateTime.Parse("2023-01-05"),
                    IsDeleted = false
                },
                new ApplicationUser
                {
                    Id = "6",
                    UserName = "bob",
                    NormalizedUserName = "BOB",
                    Email = "bob@example.com",
                    NormalizedEmail = "BOB@EXAMPLE.COM",
                    EmailConfirmed = true,
                    PasswordHash = hasher.HashPassword(new ApplicationUser(), "Test123!"),
                    SecurityStamp = Guid.NewGuid().ToString(),
                    FirstName = "Bob",
                    LastName = "Smith",
                    CreatedAt = DateTime.Parse("2023-01-06"),
                    IsDeleted = false
                },
                new ApplicationUser
                {
                    Id = "7",
                    UserName = "emma",
                    NormalizedUserName = "EMMA",
                    Email = "emma@example.com",
                    NormalizedEmail = "EMMA@EXAMPLE.COM",
                    EmailConfirmed = true,
                    PasswordHash = hasher.HashPassword(new ApplicationUser(), "Test123!"),
                    SecurityStamp = Guid.NewGuid().ToString(),
                    FirstName = "Emma",
                    LastName = "Williams",
                    CreatedAt = DateTime.Parse("2023-01-07"),
                    IsDeleted = false
                },
                new ApplicationUser
                {
                    Id = "8",
                    UserName = "michael",
                    NormalizedUserName = "MICHAEL",
                    Email = "michael@example.com",
                    NormalizedEmail = "MICHAEL@EXAMPLE.COM",
                    EmailConfirmed = true,
                    PasswordHash = hasher.HashPassword(new ApplicationUser(), "Test123!"),
                    SecurityStamp = Guid.NewGuid().ToString(),
                    FirstName = "Michael",
                    LastName = "Brown",
                    CreatedAt = DateTime.Parse("2023-01-08"),
                    IsDeleted = false
                },
                new ApplicationUser
                {
                    Id = "9",
                    UserName = "sarah",
                    NormalizedUserName = "SARAH",
                    Email = "sarah@example.com",
                    NormalizedEmail = "SARAH@EXAMPLE.COM",
                    EmailConfirmed = true,
                    PasswordHash = hasher.HashPassword(new ApplicationUser(), "Test123!"),
                    SecurityStamp = Guid.NewGuid().ToString(),
                    FirstName = "Sarah",
                    LastName = "Davis",
                    CreatedAt = DateTime.Parse("2023-01-09"),
                    IsDeleted = false
                },
                new ApplicationUser
                {
                    Id = "10",
                    UserName = "david",
                    NormalizedUserName = "DAVID",
                    Email = "david@example.com",
                    NormalizedEmail = "DAVID@EXAMPLE.COM",
                    EmailConfirmed = true,
                    PasswordHash = hasher.HashPassword(new ApplicationUser(), "Test123!"),
                    SecurityStamp = Guid.NewGuid().ToString(),
                    FirstName = "David",
                    LastName = "Miller",
                    CreatedAt = DateTime.Parse("2023-01-10"),
                    IsDeleted = false
                }
            };
            builder.Entity<ApplicationUser>().HasData(users);

            // User Roles
            builder.Entity<IdentityUserRole<string>>().HasData(
                new IdentityUserRole<string> { UserId = "1", RoleId = "2" }, // Admin
                new IdentityUserRole<string> { UserId = "2", RoleId = "3" }, // Manager
                new IdentityUserRole<string> { UserId = "3", RoleId = "1" }, // User
                new IdentityUserRole<string> { UserId = "4", RoleId = "1" }, // User
                new IdentityUserRole<string> { UserId = "5", RoleId = "1" }, // User
                new IdentityUserRole<string> { UserId = "6", RoleId = "1" }, // User
                new IdentityUserRole<string> { UserId = "7", RoleId = "1" }, // User
                new IdentityUserRole<string> { UserId = "8", RoleId = "1" }, // User
                new IdentityUserRole<string> { UserId = "9", RoleId = "1" }, // User
                new IdentityUserRole<string> { UserId = "10", RoleId = "1" }  // User
           );                                                             
        }
    }
}
