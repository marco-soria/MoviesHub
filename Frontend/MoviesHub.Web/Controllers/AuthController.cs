using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MoviesHub.Web.Models;
using MoviesHub.Web.Service.IServices;
using MoviesHub.Web.Services.IServices;
using MoviesHub.Web.Utility;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace MoviesHub.Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly ITokenProvider _tokenProvider;

        public AuthController(IAuthService authService, ITokenProvider tokenProvider)
        {
            _authService = authService;
            _tokenProvider = tokenProvider;
        }

        [HttpGet]
        public IActionResult Login()
        {
            LoginRequestDto loginRequestDto = new LoginRequestDto();
            return View(loginRequestDto);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginRequestDto loginRequestDto)
        {
            ResponseDto responseDto = await _authService.LoginAsync(loginRequestDto);

            if (responseDto != null && responseDto.IsSuccess)
            {
                LoginResponseDto loginResponseDto =
                    JsonConvert.DeserializeObject<LoginResponseDto>(Convert.ToString(responseDto.Result));

                await SignUser(loginResponseDto);
                _tokenProvider.SetToken(loginResponseDto.Token);

                return RedirectToAction("Index", "Home");
            }
            else
            {
                TempData["error"] = responseDto.Message;
                return View(loginRequestDto);
            }
        }

        [HttpGet]
        public IActionResult Register()
        {
            var roleList = new List<SelectListItem>
         {
             new SelectListItem()
             {
                 Text = SD.RoleAdmin,
                 Value = SD.RoleAdmin,
             },
             new SelectListItem()
             {
                 Text = SD.RoleManager,
                 Value = SD.RoleManager,
             },
             new SelectListItem()
             {
                 Text = SD.RoleUser,
                 Value = SD.RoleUser,
             },
         };

            ViewBag.RoleList = roleList;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegistrationRequestDto registrationRequestDto)
        {
            ResponseDto responseDto = await _authService.RegisterAsync(registrationRequestDto);

            ResponseDto assignRole;

            if (responseDto != null && responseDto.IsSuccess)
            {
                if (string.IsNullOrEmpty(registrationRequestDto.Role))
                {
                    registrationRequestDto.Role = SD.RoleAdmin;
                }
                assignRole = await _authService.AssignRoleAsync(registrationRequestDto);
                if (assignRole != null && assignRole.IsSuccess)
                {
                    TempData["success"] = "Registro exitoso";
                    return RedirectToAction(nameof(Login));
                }
            }
            else
            {
                TempData["error"] = responseDto.Message;
            }

            var roleList = new List<SelectListItem>
         {
            new SelectListItem()
             {
                 Text = SD.RoleAdmin,
                 Value = SD.RoleAdmin,
             },
             new SelectListItem()
             {
                 Text = SD.RoleManager,
                 Value = SD.RoleManager,
             },
             new SelectListItem()
             {
                 Text = SD.RoleUser,
                 Value = SD.RoleUser,
             },
         };

            ViewBag.RoleList = roleList;
            return View(registrationRequestDto);

        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            _tokenProvider.ClearToken();
            return RedirectToAction("Index", "Home");
        }





        private async Task SignUser(LoginResponseDto loginResponseDto)
        {
            var handler = new JwtSecurityTokenHandler();

            var jwt = handler.ReadJwtToken(loginResponseDto.Token);

            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);

            //standard claims
            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Email,
                jwt.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Email).Value));
            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Sub,
                jwt.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Sub).Value));
            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Name,
                jwt.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Name).Value));

            // Agregar claims para FirstName y LastName
            var firstNameClaim = jwt.Claims.FirstOrDefault(x => x.Type == "firstName");
            var lastNameClaim = jwt.Claims.FirstOrDefault(x => x.Type == "lastName");

            if (firstNameClaim != null)
            {
                identity.AddClaim(new Claim("firstName", firstNameClaim.Value));
            }

            if (lastNameClaim != null)
            {
                identity.AddClaim(new Claim("lastName", lastNameClaim.Value));
            }

            // Asignar el email como ClaimTypes.Name (para User.Identity.Name)
            identity.AddClaim(new Claim(ClaimTypes.Name,
                jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Email).Value));

            //identity.AddClaim(new Claim(ClaimTypes.Role,
            //   jwt.Claims.FirstOrDefault(x => x.Type == "role").Value));
            // Agregar roles
            var roleClaims = jwt.Claims.Where(x => x.Type == "role" || x.Type == ClaimTypes.Role);
            foreach (var roleClaim in roleClaims)
            {
                identity.AddClaim(new Claim(ClaimTypes.Role, roleClaim.Value));
            }

            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        }
    }

}
