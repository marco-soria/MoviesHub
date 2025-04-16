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
    }
}
