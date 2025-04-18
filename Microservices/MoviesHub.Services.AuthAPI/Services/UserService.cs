
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MoviesHub.Services.AuthAPI.Data;
using MoviesHub.Services.AuthAPI.Models;
using MoviesHub.Services.AuthAPI.Models.Dto;
using MoviesHub.Services.AuthAPI.Services.IServices;

namespace MoviesHub.Services.AuthAPI.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly AuthDbContext _dbContext;

        public UserService(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            AuthDbContext dbContext)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _dbContext = dbContext;
        }

        public async Task<List<UserResponseDto>> GetAllUsersAsync(bool includeDeleted = false)
        {
            IQueryable<ApplicationUser> query = _dbContext.Users;
            
            // If includeDeleted is true, we need to bypass the global query filter
            if (includeDeleted)
            {
                query = query.IgnoreQueryFilters();
            }

            var users = await query.ToListAsync();
            var userDtos = new List<UserResponseDto>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userDtos.Add(new UserResponseDto
                {
                    Id = user.Id,
                    Email = user.Email!,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    PhoneNumber = user.PhoneNumber,
                    CreatedAt = user.CreatedAt,
                    IsDeleted = user.IsDeleted,
                    DeletedAt = user.DeletedAt,
                    Roles = roles.ToList()
                });
            }

            return userDtos;
        }

        public async Task<UserResponseDto?> GetUserByIdAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return null;
            }

            var roles = await _userManager.GetRolesAsync(user);
            return new UserResponseDto
            {
                Id = user.Id,
                Email = user.Email!,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                CreatedAt = user.CreatedAt,
                IsDeleted = user.IsDeleted,
                DeletedAt = user.DeletedAt,
                Roles = roles.ToList()
            };
        }

        public async Task<UserResponseDto?> GetUserByEmailAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return null;
            }

            var roles = await _userManager.GetRolesAsync(user);
            return new UserResponseDto
            {
                Id = user.Id,
                Email = user.Email!,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                CreatedAt = user.CreatedAt,
                IsDeleted = user.IsDeleted,
                DeletedAt = user.DeletedAt,
                Roles = roles.ToList()
            };
        }

        public async Task<UserResponseDto> CreateUserAsync(UserRequestDto userRequestDto)
        {
            var user = new ApplicationUser
            {
                UserName = userRequestDto.Email,
                Email = userRequestDto.Email,
                NormalizedEmail = userRequestDto.Email.ToUpper(),
                FirstName = userRequestDto.FirstName,
                LastName = userRequestDto.LastName,
                PhoneNumber = userRequestDto.PhoneNumber,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            var result = await _userManager.CreateAsync(user, userRequestDto.Password ?? "Pass123!");
            if (!result.Succeeded)
            {
                throw new InvalidOperationException($"Failed to create user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }

            // Assign role if provided
            if (!string.IsNullOrEmpty(userRequestDto.Role))
            {
                if (!await _roleManager.RoleExistsAsync(userRequestDto.Role))
                {
                    await _roleManager.CreateAsync(new IdentityRole(userRequestDto.Role));
                }

                await _userManager.AddToRoleAsync(user, userRequestDto.Role);
            }
            else
            {
                // Default role is "User" if not specified
                if (!await _roleManager.RoleExistsAsync("User"))
                {
                    await _roleManager.CreateAsync(new IdentityRole("User"));
                }
                await _userManager.AddToRoleAsync(user, "User");
            }

            var roles = await _userManager.GetRolesAsync(user);

            return new UserResponseDto
            {
                Id = user.Id,
                Email = user.Email!,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                CreatedAt = user.CreatedAt,
                IsDeleted = user.IsDeleted,
                DeletedAt = user.DeletedAt,
                Roles = roles.ToList()
            };
        }

        public async Task<UserResponseDto?> UpdateUserAsync(string id, UserRequestDto userRequestDto)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return null;
            }

            user.Email = userRequestDto.Email;
            user.UserName = userRequestDto.Email;
            user.NormalizedEmail = userRequestDto.Email.ToUpper();
            user.FirstName = userRequestDto.FirstName;
            user.LastName = userRequestDto.LastName;
            user.PhoneNumber = userRequestDto.PhoneNumber;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException($"Failed to update user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }

            // Update password if provided
            if (!string.IsNullOrEmpty(userRequestDto.Password))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                await _userManager.ResetPasswordAsync(user, token, userRequestDto.Password);
            }

            // Update role if provided
            if (!string.IsNullOrEmpty(userRequestDto.Role))
            {
                var userRoles = await _userManager.GetRolesAsync(user);
                await _userManager.RemoveFromRolesAsync(user, userRoles);

                if (!await _roleManager.RoleExistsAsync(userRequestDto.Role))
                {
                    await _roleManager.CreateAsync(new IdentityRole(userRequestDto.Role));
                }

                await _userManager.AddToRoleAsync(user, userRequestDto.Role);
            }

            var roles = await _userManager.GetRolesAsync(user);
            return new UserResponseDto
            {
                Id = user.Id,
                Email = user.Email!,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                CreatedAt = user.CreatedAt,
                IsDeleted = user.IsDeleted,
                DeletedAt = user.DeletedAt,
                Roles = roles.ToList()
            };
        }

        public async Task<bool> DeleteUserAsync(string id, bool permanent = false)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return false;
            }

            if (permanent)
            {
                // Hard delete - remove from database
                var result = await _userManager.DeleteAsync(user);
                return result.Succeeded;
            }
            else
            {
                // Soft delete - update IsDeleted flag
                user.IsDeleted = true;
                user.DeletedAt = DateTime.UtcNow;
                var result = await _userManager.UpdateAsync(user);
                return result.Succeeded;
            }
        }

        public async Task<bool> RestoreUserAsync(string id)
        {
            // We need to bypass the global query filter to find deleted users
            var user = await _dbContext.Users.IgnoreQueryFilters()
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null || !user.IsDeleted)
            {
                return false;
            }

            user.IsDeleted = false;
            user.DeletedAt = null;
            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }
    }
}
