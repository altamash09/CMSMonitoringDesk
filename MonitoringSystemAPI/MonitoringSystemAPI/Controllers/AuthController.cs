using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MonitoringAPI.Models;
using MonitoringAPI.Services;

namespace MonitoringAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("login")]
        public async Task<ActionResult<ApiResponse<LoginResponseDto>>> Login([FromBody] LoginRequestDto loginRequest)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ApiResponse<LoginResponseDto>.ErrorResponse("Invalid request data"));
                }

                var result = await _authService.AuthenticateAsync(loginRequest);

                if (result.Success)
                {
                    return Ok(ApiResponse<LoginResponseDto>.SuccessResponse(result, "Login successful"));
                }

                return Unauthorized(ApiResponse<LoginResponseDto>.ErrorResponse(result.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login");
                return StatusCode(500, ApiResponse<LoginResponseDto>.ErrorResponse("Internal server error"));
            }
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<object>>> Logout()
        {
            try
            {
                var username = User.Identity?.Name;
                if (!string.IsNullOrEmpty(username))
                {
                    await _authService.LogoutAsync(username);
                }

                return Ok(ApiResponse<object>.SuccessResponse(null, "Logout successful"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during logout");
                return StatusCode(500, ApiResponse<object>.ErrorResponse("Internal server error"));
            }
        }

        [HttpPost("validate-token")]
        public async Task<ActionResult<ApiResponse<bool>>> ValidateToken([FromBody] string token)
        {
            try
            {
                var isValid = await _authService.ValidateTokenAsync(token);
                return Ok(ApiResponse<bool>.SuccessResponse(isValid));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating token");
                return StatusCode(500, ApiResponse<bool>.ErrorResponse("Internal server error"));
            }
        }
    }
}