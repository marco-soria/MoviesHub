using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MoviesHub.Services.AuthAPI.Models.Dto;
using MoviesHub.Services.AuthAPI.Services.IServices;

namespace MoviesHub.Services.AuthAPI.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthAPIController : ControllerBase
    {
        private readonly IAuthService _authService;
        protected ResponseDto _responseDto;

        public AuthAPIController(IAuthService authService)
        {
            _authService = authService;
            _responseDto = new ResponseDto();
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegistrationRequestDto registrationRequestDto)
        {
            var errorMessage = await _authService.Register(registrationRequestDto);
            if (!string.IsNullOrEmpty(errorMessage))
            {
                _responseDto.IsSuccess = true;
                _responseDto.Message = errorMessage;
                return BadRequest(_responseDto);
            }

            return Ok(_responseDto);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequestDto)
        {
            var loginResponse = await _authService.Login(loginRequestDto);
            if (loginResponse.User == null)
            {
                _responseDto.IsSuccess = false;
                _responseDto.Message = "Incorrect credentials";
                return BadRequest(_responseDto);
            }
            _responseDto.Result = loginResponse;
            return Ok(_responseDto);
        }

        [HttpPost("assignrole")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> AssignRole([FromBody] RegistrationRequestDto registrationRequestDto)
        {
            var assignRole = await _authService.AssignRole(registrationRequestDto.Email, registrationRequestDto.Role);
            if (!assignRole)
            {
                _responseDto.IsSuccess = false;
                _responseDto.Message = "The role could not be assigned";
                return BadRequest(_responseDto);
            }

            return Ok(_responseDto);
        }

        [HttpPost("assignrolewithDto")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> AssignRoleWithDto([FromBody] RoleAssignmentDto roleAssignmentDto)
        {
            if (!ModelState.IsValid)
            {
                _responseDto.IsSuccess = false;
                _responseDto.Message = "Información de asignación de rol incompleta";
                return BadRequest(_responseDto);
            }

            var assignRole = await _authService.AssignRoleWithDto(roleAssignmentDto);
            if (!assignRole)
            {
                _responseDto.IsSuccess = false;
                _responseDto.Message = "No se pudo asignar el rol";
                return BadRequest(_responseDto);
            }

            _responseDto.IsSuccess = true;
            _responseDto.Message = "Rol asignado exitosamente";
            return Ok(_responseDto);
        }

        [HttpGet("users-with-roles")]
        public async Task<IActionResult> GetUsersWithRoles()
        {
            var usersWithRoles = await _authService.GetUsersWithRoles();
            _responseDto.Result = usersWithRoles;
            _responseDto.IsSuccess = true;
            return Ok(_responseDto);
        }



    }
}
