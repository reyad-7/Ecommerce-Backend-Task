using Domain.DTOS.Auth;
using Domain.Interfaces.IAuthService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static Domain.Entities.GeneralResponse.GeneralResponse;

namespace E_Commerce.Controllers.AuthController
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthServie _authService;
        public AuthController(IAuthServie authService)
        {
            _authService = authService;
        }
        /// <summary>
        /// Register a new user
        /// </summary>
        /// <param name="dto">Registration data</param>
        /// <returns>Auth response with JWT token</returns>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(GeneralResponseDto<AuthResponseDto>.FailureResponse(
                    "Invalid input data. Please check your request."
                ));
            }

            var response = await _authService.RegisterAsync(dto);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        /// <summary>
        /// Login with email/username and password
        /// </summary>
        /// <param name="dto">Login credentials</param>
        /// <returns>Auth response with JWT token</returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(GeneralResponseDto<AuthResponseDto>.FailureResponse(
                    "Invalid input data. Please check your request."
                ));
            }
            var response = await _authService.LoginAsync(dto);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }
    }
}
