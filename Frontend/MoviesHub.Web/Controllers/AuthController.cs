using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using MoviesHub.Web.Models;
using MoviesHub.Web.Services.IServices;
using MoviesHub.Web.Utility;
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
                var resultObject = responseDto.Result as Newtonsoft.Json.Linq.JObject;
                var innerResult = resultObject["result"] as Newtonsoft.Json.Linq.JObject;

                LoginResponseDto loginResponseDto = new LoginResponseDto
                {
                    User = innerResult["user"]?.ToObject<UserDto>(),
                    Token = innerResult["token"]?.ToString()
                };

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

        //[HttpPost]
        //public async Task<IActionResult> Login(LoginRequestDto loginRequestDto)
        //{
        //    ResponseDto responseDto = await _authService.LoginAsync(loginRequestDto);

        //    if (responseDto != null && responseDto.IsSuccess)
        //    {
        //        try
        //        {
        //            // Veamos exactamente qué contiene responseDto.Result
        //            var resultJson = JsonConvert.SerializeObject(responseDto.Result);
        //            Console.WriteLine($"ResponseDto.Result JSON: {resultJson}");

        //            var resultObject = responseDto.Result as Newtonsoft.Json.Linq.JObject;

        //            // Veamos todas las propiedades disponibles en el JObject
        //            Console.WriteLine("Propiedades disponibles en resultObject:");
        //            foreach (var property in resultObject.Properties())
        //            {
        //                Console.WriteLine($"- {property.Name}: {property.Value?.ToString()?.Substring(0, Math.Min(20, property.Value?.ToString().Length ?? 0))}...");
        //            }

        //            // Veamos si hay una propiedad "result" dentro de resultObject y qué contiene
        //            if (resultObject["result"] != null)
        //            {
        //                Console.WriteLine("Propiedades dentro de resultObject['result']:");
        //                var innerResult = resultObject["result"] as Newtonsoft.Json.Linq.JObject;
        //                if (innerResult != null)
        //                {
        //                    foreach (var property in innerResult.Properties())
        //                    {
        //                        Console.WriteLine($"- {property.Name}: {property.Value?.ToString()?.Substring(0, Math.Min(20, property.Value?.ToString().Length ?? 0))}...");
        //                    }
        //                }
        //            }

        //            // Intentar acceder al token de varias maneras posibles
        //            string token = null;

        //            // Intento 1: Directo del objeto principal
        //            token = resultObject["token"]?.ToString();
        //            Console.WriteLine($"Intento 1 (token directo): {token?.Substring(0, Math.Min(10, token?.Length ?? 0)) ?? "null"}");

        //            // Intento 2: A través de la propiedad result
        //            if (token == null && resultObject["result"] != null)
        //            {
        //                token = resultObject["result"]["token"]?.ToString();
        //                Console.WriteLine($"Intento 2 (token en result): {token?.Substring(0, Math.Min(10, token?.Length ?? 0)) ?? "null"}");
        //            }

        //            // Crear el LoginResponseDto con el token encontrado
        //            LoginResponseDto loginResponseDto = new LoginResponseDto();

        //            // Determinar cómo extraer el usuario según donde se encontró el token
        //            if (token != null)
        //            {
        //                loginResponseDto.Token = token;

        //                // Intentar obtener el usuario de manera similar
        //                if (resultObject["user"] != null)
        //                {
        //                    loginResponseDto.User = resultObject["user"].ToObject<UserDto>();
        //                }
        //                else if (resultObject["result"] != null && resultObject["result"]["user"] != null)
        //                {
        //                    loginResponseDto.User = resultObject["result"]["user"].ToObject<UserDto>();
        //                }
        //            }
        //            else
        //            {
        //                Console.WriteLine("No se pudo encontrar el token en ninguna ubicación esperada");
        //                TempData["error"] = "Error en la autenticación: token no encontrado";
        //                return View(loginRequestDto);
        //            }

        //            await SignUser(loginResponseDto);
        //            _tokenProvider.SetToken(loginResponseDto.Token);

        //            return RedirectToAction("Index", "Home");
        //        }
        //        catch (Exception ex)
        //        {
        //            Console.WriteLine($"Error en deserialización: {ex.Message}");
        //            Console.WriteLine($"StackTrace: {ex.StackTrace}");
        //            TempData["error"] = "Error en la autenticación: " + ex.Message;
        //            return View(loginRequestDto);
        //        }
        //    }
        //    else
        //    {
        //        TempData["error"] = responseDto.Message;
        //        return View(loginRequestDto);
        //    }
        //}



        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegistrationRequestDto registrationRequestDto)
        {
            // Asignar el rol User por defecto
            registrationRequestDto.Role = SD.RoleUser;

            ResponseDto responseDto = await _authService.RegisterAsync(registrationRequestDto);

            if (responseDto != null && responseDto.IsSuccess)
            {
                // Asignar el rol
                ResponseDto assignRole = await _authService.AssignRoleAsync(registrationRequestDto);

                if (assignRole != null && assignRole.IsSuccess)
                {
                    TempData["success"] = "Registro exitoso";
                    return RedirectToAction(nameof(Login));
                }
            }

            TempData["error"] = responseDto?.Message ?? "Error al registrar el usuario";
            return View(registrationRequestDto);
        }


        //[HttpGet]
        //public IActionResult Register()
        //{
        //    var roleList = new List<SelectListItem>
        // {
        //     new SelectListItem()
        //     {
        //         Text = SD.RoleAdmin,
        //         Value = SD.RoleAdmin,
        //     },
        //     new SelectListItem()
        //     {
        //         Text = SD.RoleManager,
        //         Value = SD.RoleManager,
        //     },
        //     new SelectListItem()
        //     {
        //         Text = SD.RoleUser,
        //         Value = SD.RoleUser,
        //     },
        // };

        //    ViewBag.RoleList = roleList;
        //    return View();
        //}

        //[HttpPost]
        //public async Task<IActionResult> Register(RegistrationRequestDto registrationRequestDto)
        //{
        //    ResponseDto responseDto = await _authService.RegisterAsync(registrationRequestDto);

        //    ResponseDto assignRole;

        //    if (responseDto != null && responseDto.IsSuccess)
        //    {
        //        if (string.IsNullOrEmpty(registrationRequestDto.Role))
        //        {
        //            registrationRequestDto.Role = SD.RoleAdmin;
        //        }
        //        assignRole = await _authService.AssignRoleAsync(registrationRequestDto);
        //        if (assignRole != null && assignRole.IsSuccess)
        //        {
        //            TempData["success"] = "Registro exitoso";
        //            return RedirectToAction(nameof(Login));
        //        }
        //    }
        //    else
        //    {
        //        TempData["error"] = responseDto.Message;
        //    }

        //    var roleList = new List<SelectListItem>
        // {
        //    new SelectListItem()
        //     {
        //         Text = SD.RoleAdmin,
        //         Value = SD.RoleAdmin,
        //     },
        //     new SelectListItem()
        //     {
        //         Text = SD.RoleManager,
        //         Value = SD.RoleManager,
        //     },
        //     new SelectListItem()
        //     {
        //         Text = SD.RoleUser,
        //         Value = SD.RoleUser,
        //     },
        // };

        //    ViewBag.RoleList = roleList;
        //    return View(registrationRequestDto);

        //}

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            _tokenProvider.ClearToken();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }



        [HttpGet]
        public IActionResult AssignRole()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AssignRole(RoleAssignmentDto roleAssignmentDto)
        {
            if (!ModelState.IsValid)
            {
                return View(roleAssignmentDto);
            }

            try
            {
                // Usar el nuevo método con DTO específico
                ResponseDto responseDto = await _authService.AssignRoleWithDtoAsync(roleAssignmentDto);

                if (responseDto != null && responseDto.IsSuccess)
                {
                    TempData["success"] = "Rol asignado correctamente";
                }
                else
                {
                    TempData["error"] = responseDto?.Message ?? "Error al asignar el rol";
                }
            }
            catch (Exception ex)
            {
                TempData["error"] = $"Error: {ex.Message}";
                // Log the exception details
                Console.WriteLine($"Exception: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }

            return View(roleAssignmentDto);
        }


        //    private async Task SignUser(LoginResponseDto loginResponseDto)
        //    {
        //        var handler = new JwtSecurityTokenHandler();

        //        var jwt = handler.ReadJwtToken(loginResponseDto.Token);

        //        var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);

        //        //standard claims
        //        identity.AddClaim(new Claim(JwtRegisteredClaimNames.Email,
        //            jwt.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Email).Value));
        //        identity.AddClaim(new Claim(JwtRegisteredClaimNames.Sub,
        //            jwt.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Sub).Value));
        //        identity.AddClaim(new Claim(JwtRegisteredClaimNames.Name,
        //            jwt.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Name).Value));

        //        // Agregar claims para FirstName y LastName
        //        var firstNameClaim = jwt.Claims.FirstOrDefault(x => x.Type == "firstName");
        //        var lastNameClaim = jwt.Claims.FirstOrDefault(x => x.Type == "lastName");

        //        if (firstNameClaim != null)
        //        {
        //            identity.AddClaim(new Claim("firstName", firstNameClaim.Value));
        //        }

        //        if (lastNameClaim != null)
        //        {
        //            identity.AddClaim(new Claim("lastName", lastNameClaim.Value));
        //        }

        //        // Asignar el email como ClaimTypes.Name (para User.Identity.Name)
        //        identity.AddClaim(new Claim(ClaimTypes.Name,
        //            jwt.Claims.FirstOrDefault(u => u.Type == JwtRegisteredClaimNames.Email).Value));

        //        //identity.AddClaim(new Claim(ClaimTypes.Role,
        //        //   jwt.Claims.FirstOrDefault(x => x.Type == "role").Value));
        //        // Agregar roles
        //        var roleClaims = jwt.Claims.Where(x => x.Type == "role" || x.Type == ClaimTypes.Role);
        //        foreach (var roleClaim in roleClaims)
        //        {
        //            identity.AddClaim(new Claim(ClaimTypes.Role, roleClaim.Value));
        //        }

        //        var principal = new ClaimsPrincipal(identity);

        //        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        //    }
        //}
        private async Task SignUser(LoginResponseDto loginResponseDto)
        {
            var handler = new JwtSecurityTokenHandler();

            try
            {
                var jwt = handler.ReadJwtToken(loginResponseDto.Token);

                var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);

                // El claim sub contiene el ID del usuario - IMPORTANTE
                var userIdClaim = jwt.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Sub);
                if (userIdClaim != null)
                {
                    // Agregar explícitamente como NameIdentifier para User.FindFirst(ClaimTypes.NameIdentifier)
                    identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, userIdClaim.Value));
                }

                // Email
                var emailClaim = jwt.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Email);
                if (emailClaim != null)
                {
                    identity.AddClaim(new Claim(JwtRegisteredClaimNames.Email, emailClaim.Value));
                    identity.AddClaim(new Claim(ClaimTypes.Name, emailClaim.Value)); // Para User.Identity.Name
                }

                // Nombre de usuario
                var nameClaim = jwt.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Name);
                if (nameClaim != null)
                {
                    identity.AddClaim(new Claim(JwtRegisteredClaimNames.Name, nameClaim.Value));
                }

                // FirstName y LastName
                var firstNameClaim = jwt.Claims.FirstOrDefault(x => x.Type == "firstName");
                if (firstNameClaim != null)
                {
                    identity.AddClaim(new Claim("firstName", firstNameClaim.Value));
                }

                var lastNameClaim = jwt.Claims.FirstOrDefault(x => x.Type == "lastName");
                if (lastNameClaim != null)
                {
                    identity.AddClaim(new Claim("lastName", lastNameClaim.Value));
                }

                // Roles
                var roleClaims = jwt.Claims.Where(x => x.Type == "role" || x.Type == ClaimTypes.Role);
                foreach (var roleClaim in roleClaims)
                {
                    identity.AddClaim(new Claim(ClaimTypes.Role, roleClaim.Value));
                }

                var principal = new ClaimsPrincipal(identity);

                // Log de diagnóstico
                Console.WriteLine($"Usuario autenticado. ID: {userIdClaim?.Value}, Email: {emailClaim?.Value}");

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en SignUser: {ex.Message}");
                throw;
            }
        }



    }
}
