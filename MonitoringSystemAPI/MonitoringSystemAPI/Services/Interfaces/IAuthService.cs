using MonitoringAPI.Models;

namespace MonitoringAPI.Services
{
    public interface IAuthService
    {
        Task<LoginResponseDto> AuthenticateAsync(LoginRequestDto loginRequest);
        Task<bool> ValidateTokenAsync(string token);
        Task LogoutAsync(string username);
    }
}