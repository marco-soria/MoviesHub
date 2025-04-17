using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoviesHub.Web.Models;
using MoviesHub.Web.Services.IServices;
using MoviesHub.Web.Utility;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace MoviesHub.Web.Controllers
{
    //[Authorize(Roles = SD.RoleAdmin)]
    public class RoleController : Controller
    {
        private readonly IAuthService _authService;

        public RoleController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpGet]
        public async Task<IActionResult> AssignRole()
        {
            // Obtener la lista de usuarios con sus roles
            var responseDto = await _authService.GetUsersWithRolesAsync();
            List<UserWithRoleDto> usersList = new List<UserWithRoleDto>();

            if (responseDto != null && responseDto.IsSuccess)
            {
                var resultObject = JsonConvert.DeserializeObject<dynamic>(JsonConvert.SerializeObject(responseDto.Result));
                usersList = JsonConvert.DeserializeObject<List<UserWithRoleDto>>(resultObject.result.ToString());
            }
            else
            {
                TempData["error"] = "Error al cargar los usuarios";
            }

            // Crear el ViewBag para almacenar la lista de usuarios
            ViewBag.Users = usersList;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AssignRole(RoleAssignmentDto roleAssignmentDto)
        {
            if (!ModelState.IsValid)
            {
                // Recargar la lista de usuarios en caso de error de validación
                var usersResponse = await _authService.GetUsersWithRolesAsync();
                if (usersResponse != null && usersResponse.IsSuccess)
                {
                    string resultJson = JsonConvert.SerializeObject(usersResponse.Result);
                    ViewBag.Users = JsonConvert.DeserializeObject<List<UserWithRoleDto>>(resultJson);
                }

                return View(roleAssignmentDto);
            }

            try
            {
                // Usar el método correcto que llama al nuevo endpoint
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
                Console.WriteLine($"Exception: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }

            // Recargar la página con los datos actualizados
            return RedirectToAction(nameof(AssignRole));
        }
    }
}
