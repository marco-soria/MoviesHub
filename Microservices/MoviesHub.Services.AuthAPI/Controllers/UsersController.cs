using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoviesHub.Services.AuthAPI.Models.Dto;
using MoviesHub.Services.AuthAPI.Services.IServices;

namespace MoviesHub.Services.AuthAPI.Controllers
{
    [Route("api/users")]
    [ApiController]
    //[Authorize(Roles = "Admin")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        protected ResponseDto _response;

        public UsersController(IUserService userService)
        {
            _userService = userService;
            _response = new ResponseDto();
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers([FromQuery] bool includeDeleted = false)
        {
            try
            {
                var users = await _userService.GetAllUsersAsync(includeDeleted);
                _response.Result = users;
                _response.IsSuccess = true;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
                return BadRequest(_response);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(string id)
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(id);
                if (user == null)
                {
                    _response.IsSuccess = false;
                    _response.Message = "User not found";
                    return NotFound(_response);
                }

                _response.Result = user;
                _response.IsSuccess = true;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
                return BadRequest(_response);
            }
        }

        [HttpGet("email/{email}")]
        public async Task<IActionResult> GetUserByEmail(string email)
        {
            try
            {
                var user = await _userService.GetUserByEmailAsync(email);
                if (user == null)
                {
                    _response.IsSuccess = false;
                    _response.Message = "User not found";
                    return NotFound(_response);
                }

                _response.Result = user;
                _response.IsSuccess = true;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
                return BadRequest(_response);
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> CreateUser([FromBody] UserRequestDto userRequestDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var createdUser = await _userService.CreateUserAsync(userRequestDto);
                
                _response.Result = createdUser;
                _response.IsSuccess = true;
                _response.Message = "User created successfully";
                
                return CreatedAtAction(nameof(GetUserById), new { id = createdUser.Id }, _response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
                return BadRequest(_response);
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] UserRequestDto userRequestDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var updatedUser = await _userService.UpdateUserAsync(id, userRequestDto);
                if (updatedUser == null)
                {
                    _response.IsSuccess = false;
                    _response.Message = "User not found";
                    return NotFound(_response);
                }

                _response.Result = updatedUser;
                _response.IsSuccess = true;
                _response.Message = "User updated successfully";
                
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
                return BadRequest(_response);
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> DeleteUser(string id, [FromQuery] bool permanent = false)
        {
            try
            {
                // Los logs de depuración pueden mantenerse para verificación
                var authHeader = Request.Headers["Authorization"].ToString();
                Console.WriteLine($"==== HEADERS RECIBIDOS EN DELETEUSER ====");
                foreach (var header in Request.Headers)
                {
                    Console.WriteLine($"{header.Key}: {header.Value}");
                }
                Console.WriteLine($"Auth header: {authHeader}");
                Console.WriteLine($"Usuario autenticado: {User.Identity.IsAuthenticated}");
                Console.WriteLine($"Nombre: {User.Identity.Name}");
                Console.WriteLine($"Claims:");
                foreach (var claim in User.Claims)
                {
                    Console.WriteLine($"- {claim.Type}: {claim.Value}");
                }
                Console.WriteLine($"Admin: {User.IsInRole("Admin")}, Manager: {User.IsInRole("Manager")}");
                Console.WriteLine($"====================================");

                // Restauramos la verificación de roles (opcional, ya que [Authorize] ya lo está haciendo)
                if (!User.IsInRole("Admin") && !User.IsInRole("Manager"))
                {
                    _response.IsSuccess = false;
                    _response.Message = "No tienes permisos para realizar esta acción";
                    return Forbid(); // Devuelve 403 Forbidden en vez de 401
                }

                var result = await _userService.DeleteUserAsync(id, permanent);
                if (!result)
                {
                    _response.IsSuccess = false;
                    _response.Message = "User not found";
                    return NotFound(_response);
                }

                _response.IsSuccess = true;
                _response.Message = permanent
                    ? "User permanently deleted"
                    : "User soft deleted successfully";

                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
                return StatusCode(500, _response);
            }
        }





        [HttpPost("{id}/restore")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> RestoreUser(string id)
        {
            try
            {
                var result = await _userService.RestoreUserAsync(id);
                if (!result)
                {
                    _response.IsSuccess = false;
                    _response.Message = "User not found or already active";
                    return NotFound(_response);
                }

                _response.IsSuccess = true;
                _response.Message = "User restored successfully";
                
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
                return BadRequest(_response);
            }
        }
    }
}
