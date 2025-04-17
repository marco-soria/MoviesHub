using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MoviesHub.Services.AuthAPI.Data;
using MoviesHub.Services.AuthAPI.Models;
using MoviesHub.Services.AuthAPI.Models.Dto;
using MoviesHub.Services.AuthAPI.Services.IServices;

namespace MoviesHub.Services.AuthAPI.Services
{
    public class AuthService : IAuthService
    {
        private readonly AuthDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;

        public AuthService(AuthDbContext db, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IJwtTokenGenerator jwtTokenGenerator)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
            _jwtTokenGenerator = jwtTokenGenerator;
        }
        public async Task<string> Register(RegistrationRequestDto registrationRequestDto)
        {
            ApplicationUser user = new ApplicationUser()
            {
                UserName = registrationRequestDto.Email,
                Email = registrationRequestDto.Email,
                NormalizedEmail = registrationRequestDto.Email.ToUpper(),
                FirstName = registrationRequestDto.FirstName,
                LastName = registrationRequestDto.LastName,
                PhoneNumber = registrationRequestDto.PhoneNumber,
            };

            try
            {
                var result = await _userManager.CreateAsync(user, registrationRequestDto.Password);
                if (result.Succeeded)
                {
                    var userToReturn = _db.ApplicationUsers.First(x => x.UserName == registrationRequestDto.Email);
                    UserDto userDTO = new UserDto()
                    {
                        Email = userToReturn.Email,
                        Id = userToReturn.Id,
                        FirstName = userToReturn.FirstName,
                        LastName = userToReturn.LastName,
                        PhoneNumber = userToReturn.PhoneNumber
                    };

                    return "";
                }
                else
                {
                    return result.Errors.FirstOrDefault().Description;
                }
            }
            catch (Exception ex)
            {
            }

            return "Error encontrado";
        }
        public async Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto)
        {
            var user = _db.ApplicationUsers.FirstOrDefault(x => x.UserName.ToLower() == loginRequestDto.UserName.ToLower());
            bool isValid = await _userManager.CheckPasswordAsync(user, loginRequestDto.Password);
            if (user == null || isValid == false)
            {
                return new LoginResponseDto()
                {
                    User = null,
                    Token = ""
                };
            }

            //si lo encuentra al usuario deberia general el token
            var roles = await _userManager.GetRolesAsync(user);
            var token = _jwtTokenGenerator.GenerateToken(user, roles);

            UserDto userDto = new UserDto()
            {
                Email = user.Email,
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber
            };

            LoginResponseDto loginResponseDto = new LoginResponseDto()
            {
                User = userDto,
                Token = token,
            };

            return loginResponseDto;

        }
        //public async Task<bool> AssignRole(string email, string roleName)
        //{
        //    var user = _db.ApplicationUsers.FirstOrDefault(x => x.Email.ToLower() == email.ToLower());

        //    if (user != null)
        //    {
        //        if (!_roleManager.RoleExistsAsync(roleName).GetAwaiter().GetResult())
        //        {
        //            _roleManager.CreateAsync(new IdentityRole(roleName)).GetAwaiter().GetResult();
        //        }
        //        await _userManager.AddToRoleAsync(user, roleName);
        //        return true;
        //    }
        //    return false;
        //}

        public async Task<bool> AssignRole(string email, string roleName)
        {
            var user = _db.ApplicationUsers.FirstOrDefault(x => x.Email.ToLower() == email.ToLower());

            if (user != null)
            {
                if (!_roleManager.RoleExistsAsync(roleName).GetAwaiter().GetResult())
                {
                    _roleManager.CreateAsync(new IdentityRole(roleName)).GetAwaiter().GetResult();
                }

                // Get current roles
                var currentRoles = await _userManager.GetRolesAsync(user);

                // Remove all current roles
                if (currentRoles.Any())
                {
                    await _userManager.RemoveFromRolesAsync(user, currentRoles);
                }

                // Add the new role
                await _userManager.AddToRoleAsync(user, roleName);
                return true;
            }
            return false;
        }

        // Agregar este método a la clase AuthService
        public async Task<bool> AssignRoleWithDto(RoleAssignmentDto roleAssignmentDto)
        {
            return await AssignRole(roleAssignmentDto.Email, roleAssignmentDto.Role);
        }

        public async Task<List<UserWithRoleDto>> GetUsersWithRoles()
        {
            var users = _db.ApplicationUsers.Where(u => !u.IsDeleted).ToList();
            var usersWithRoles = new List<UserWithRoleDto>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);

                var userWithRole = new UserWithRoleDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    PhoneNumber = user.PhoneNumber,
                    Roles = roles.ToList()
                };

                usersWithRoles.Add(userWithRole);
            }

            return usersWithRoles;
        }



    }
}
