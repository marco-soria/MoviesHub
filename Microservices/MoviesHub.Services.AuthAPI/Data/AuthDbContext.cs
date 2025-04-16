using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
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
        }
    }
}
