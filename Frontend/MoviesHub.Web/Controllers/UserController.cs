using Microsoft.AspNetCore.Mvc;
using MoviesHub.Web.Models;
using MoviesHub.Web.Services.IServices;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace MoviesHub.Web.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<IActionResult> Index(bool includeDeleted = false)
        {
            List<UserResponseDto> userList = new();

            try
            {
                var response = await _userService.GetAllUsersAsync(includeDeleted);

                if (response != null && response.IsSuccess)
                {
                    // Log para depuración
                    string jsonStr = JsonConvert.SerializeObject(response.Result);
                    Console.WriteLine($"Index - JSON recibido: {jsonStr}");

                    try
                    {
                        // Intentar deserializar directamente
                        userList = JsonConvert.DeserializeObject<List<UserResponseDto>>(jsonStr);

                        // Verificar si los datos son válidos
                        if (userList.Count > 0 && userList.Any(u => u.Email == null || u.FirstName == null))
                        {
                            userList = new List<UserResponseDto>(); // Resetear para intentar otro método
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Index - Error en primera deserialización: {ex.Message}");
                    }

                    // Si el primer método falló, intentamos con el segundo
                    if (userList.Count == 0)
                    {
                        try
                        {
                            var jObj = JObject.Parse(jsonStr);

                            // Depuración: ver qué propiedades tiene el objeto
                            foreach (var prop in jObj.Properties())
                            {
                                Console.WriteLine($"Index - Propiedad: {prop.Name}, Tipo: {prop.Value?.Type}");
                            }

                            // Intentar obtener el resultado del objeto anidado
                            if (jObj.TryGetValue("result", out JToken resultToken))
                            {
                                userList = JsonConvert.DeserializeObject<List<UserResponseDto>>(resultToken.ToString());
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Index - Error en segunda deserialización: {ex.Message}");
                            TempData["error"] = $"Error procesando lista de usuarios: {ex.Message}";
                        }
                    }

                    ViewBag.IncludeDeleted = includeDeleted;
                }
                else
                {
                    TempData["error"] = response?.Message ?? "Error al obtener usuarios";
                }
            }
            catch (Exception ex)
            {
                TempData["error"] = $"Error: {ex.Message}";
                Console.WriteLine($"Index - Exception: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }

            return View(userList);
        }

        public async Task<IActionResult> Details(string id)
        {
            try
            {
                var response = await _userService.GetUserByIdAsync(id);

                if (response != null && response.IsSuccess)
                {
                    string jsonStr = JsonConvert.SerializeObject(response.Result);
                    Console.WriteLine($"JSON recibido: {jsonStr}"); // Log para depuración

                    UserResponseDto user = null;

                    try
                    {
                        // Intentar deserializar primero directamente
                        user = JsonConvert.DeserializeObject<UserResponseDto>(jsonStr);

                        // Si el objeto deserializado tiene propiedades nulas que no deberían serlo, es que está mal
                        if (user?.Email == null || user?.FirstName == null)
                        {
                            user = null; // Lo anulamos para intentar otro método
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error en primera deserialización: {ex.Message}");
                    }

                    // Si el primer método falló, intentamos con el segundo
                    if (user == null)
                    {
                        try
                        {
                            var jObj = JObject.Parse(jsonStr);

                            // Depuración: ver qué propiedades tiene el objeto
                            foreach (var prop in jObj.Properties())
                            {
                                Console.WriteLine($"Propiedad: {prop.Name}, Tipo: {prop.Value?.Type}");
                            }

                            // Intentar obtener el resultado del objeto anidado
                            if (jObj.TryGetValue("result", out JToken resultToken))
                            {
                                user = JsonConvert.DeserializeObject<UserResponseDto>(resultToken.ToString());
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error en segunda deserialización: {ex.Message}");
                            TempData["error"] = $"Error procesando datos: {ex.Message}";
                        }
                    }

                    if (user != null)
                    {
                        return View(user);
                    }
                    else
                    {
                        TempData["error"] = "No se pudo obtener los datos del usuario";
                        return RedirectToAction(nameof(Index));
                    }
                }

                TempData["error"] = response?.Message ?? "Usuario no encontrado";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["error"] = $"Error: {ex.Message}";
                Console.WriteLine($"Exception en Details: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return RedirectToAction(nameof(Index));
            }
        }


        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserRequestDto model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var response = await _userService.CreateUserAsync(model);

                    if (response != null && response.IsSuccess)
                    {
                        TempData["success"] = "Usuario creado exitosamente";
                        return RedirectToAction(nameof(Index));
                    }

                    TempData["error"] = response?.Message ?? "Error al crear usuario";
                }
                catch (Exception ex)
                {
                    TempData["error"] = $"Error: {ex.Message}";
                    Console.WriteLine($"Exception: {ex.Message}");
                    Console.WriteLine($"Stack trace: {ex.StackTrace}");
                }
            }

            return View(model);
        }

        public async Task<IActionResult> Edit(string id)
        {
            try
            {
                var response = await _userService.GetUserByIdAsync(id);

                if (response != null && response.IsSuccess)
                {
                    string jsonStr = JsonConvert.SerializeObject(response.Result);
                    Console.WriteLine($"Edit (GET) - JSON recibido: {jsonStr}");

                    UserResponseDto userResponse = null;

                    try
                    {
                        // Intentar deserializar primero directamente
                        userResponse = JsonConvert.DeserializeObject<UserResponseDto>(jsonStr);

                        // Si el objeto deserializado tiene propiedades nulas que no deberían serlo, es que está mal
                        if (userResponse?.Email == null || userResponse?.FirstName == null)
                        {
                            userResponse = null; // Lo anulamos para intentar otro método
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Edit (GET) - Error en primera deserialización: {ex.Message}");
                    }

                    // Si el primer método falló, intentamos con el segundo
                    if (userResponse == null)
                    {
                        try
                        {
                            var jObj = JObject.Parse(jsonStr);

                            // Intentar obtener el resultado del objeto anidado
                            if (jObj.TryGetValue("result", out JToken resultToken))
                            {
                                userResponse = JsonConvert.DeserializeObject<UserResponseDto>(resultToken.ToString());
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Edit (GET) - Error en segunda deserialización: {ex.Message}");
                        }
                    }

                    if (userResponse == null)
                    {
                        TempData["error"] = "Error al procesar datos del usuario";
                        return RedirectToAction(nameof(Index));
                    }

                    // Map to UserRequestDto
                    var userRequestDto = new UserRequestDto
                    {
                        Email = userResponse.Email,
                        FirstName = userResponse.FirstName,
                        LastName = userResponse.LastName,
                        PhoneNumber = userResponse.PhoneNumber,
                        Role = userResponse.Roles.FirstOrDefault()
                    };

                    return View(userRequestDto);
                }

                TempData["error"] = response?.Message ?? "Usuario no encontrado";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["error"] = $"Error: {ex.Message}";
                Console.WriteLine($"Edit (GET) - Exception: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, UserRequestDto model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var response = await _userService.UpdateUserAsync(id, model);

                    if (response != null && response.IsSuccess)
                    {
                        TempData["success"] = "Usuario actualizado exitosamente";
                        return RedirectToAction(nameof(Index));
                    }

                    TempData["error"] = response?.Message ?? "Error al actualizar usuario";
                }
                catch (Exception ex)
                {
                    TempData["error"] = $"Error: {ex.Message}";
                    Console.WriteLine($"Exception: {ex.Message}");
                    Console.WriteLine($"Stack trace: {ex.StackTrace}");
                }
            }

            return View(model);
        }

        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var response = await _userService.GetUserByIdAsync(id);

                if (response != null && response.IsSuccess)
                {
                    string jsonStr = JsonConvert.SerializeObject(response.Result);
                    Console.WriteLine($"Delete (GET) - JSON recibido: {jsonStr}");

                    UserResponseDto user = null;

                    try
                    {
                        // Intentar deserializar primero directamente
                        user = JsonConvert.DeserializeObject<UserResponseDto>(jsonStr);

                        // Si el objeto deserializado tiene propiedades nulas que no deberían serlo, es que está mal
                        if (user?.Email == null || user?.FirstName == null)
                        {
                            user = null; // Lo anulamos para intentar otro método
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Delete (GET) - Error en primera deserialización: {ex.Message}");
                    }

                    // Si el primer método falló, intentamos con el segundo
                    if (user == null)
                    {
                        try
                        {
                            var jObj = JObject.Parse(jsonStr);

                            // Intentar obtener el resultado del objeto anidado
                            if (jObj.TryGetValue("result", out JToken resultToken))
                            {
                                user = JsonConvert.DeserializeObject<UserResponseDto>(resultToken.ToString());
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Delete (GET) - Error en segunda deserialización: {ex.Message}");
                        }
                    }

                    if (user != null)
                    {
                        return View(user);
                    }
                    else
                    {
                        TempData["error"] = "No se pudo obtener los datos del usuario para eliminar";
                        return RedirectToAction(nameof(Index));
                    }
                }

                TempData["error"] = response?.Message ?? "Usuario no encontrado";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["error"] = $"Error: {ex.Message}";
                Console.WriteLine($"Delete (GET) - Exception: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(UserDeleteDto model)
        {
            try
            {
                var response = await _userService.DeleteUserAsync(model.Id, model.Permanent);

                if (response != null && response.IsSuccess)
                {
                    TempData["success"] = model.Permanent
                        ? "Usuario eliminado permanentemente"
                        : "Usuario desactivado exitosamente";
                }
                else
                {
                    TempData["error"] = response?.Message ?? "Error al eliminar usuario";
                }
            }
            catch (Exception ex)
            {
                TempData["error"] = $"Error: {ex.Message}";
                Console.WriteLine($"Exception: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Restore(string id)
        {
            try
            {
                var response = await _userService.RestoreUserAsync(id);

                if (response != null && response.IsSuccess)
                {
                    TempData["success"] = "Usuario restaurado exitosamente";
                }
                else
                {
                    TempData["error"] = response?.Message ?? "Error al restaurar usuario";
                }
            }
            catch (Exception ex)
            {
                TempData["error"] = $"Error: {ex.Message}";
                Console.WriteLine($"Exception: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }

            return RedirectToAction(nameof(Index), new { includeDeleted = true });
        }
    }
}
