using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using System.Data;
using MonitoringAPI.Models;
using MonitoringAPI.Data;
using MonitoringAPI.Services;

namespace MonitoringSystemAPI.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly MonitoringDbContext _context;
        private readonly ILogger<AuthService> _logger;
        private readonly IConfiguration _configuration;

        public AuthService(MonitoringDbContext context, ILogger<AuthService> logger, IConfiguration configuration)
        {
            _context = context;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<LoginResponseDto> AuthenticateAsync(LoginRequestDto loginRequest)
        {
            try
            {
                using var command = _context.Database.GetDbConnection().CreateCommand();
                command.CommandText = "sp_AuthenticateUser";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@Username", loginRequest.Username));
                command.Parameters.Add(new SqlParameter("@Password", loginRequest.Password));

                await _context.Database.OpenConnectionAsync();
                using var reader = await command.ExecuteReaderAsync();

                if (await reader.ReadAsync())
                {
                    var userId = reader.GetInt32("Id");
                    var username = reader.GetString("Username");
                    var email = reader.IsDBNull("Email") ? null : reader.GetString("Email");
                    var isValid = reader.GetBoolean("IsValid");

                    if (isValid)
                    {
                        // Generate JWT token (simplified - you'd use proper JWT library)
                        var token = GenerateJwtToken(userId, username);

                        // Update last login
                        await UpdateLastLoginAsync(userId);

                        return new LoginResponseDto
                        {
                            Success = true,
                            Token = token,
                            Message = "Login successful",
                            User = new UserInfoDto
                            {
                                Id = userId,
                                Username = username,
                                Email = email
                            }
                        };
                    }
                }

                return new LoginResponseDto
                {
                    Success = false,
                    Message = "Invalid username or password"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during authentication");
                return new LoginResponseDto
                {
                    Success = false,
                    Message = "Authentication failed"
                };
            }
            finally
            {
                await _context.Database.CloseConnectionAsync();
            }
        }

        public async Task<bool> ValidateTokenAsync(string token)
        {
            try
            {
                // Implement JWT token validation logic here
                // For now, returning true as placeholder
                await Task.CompletedTask;
                return !string.IsNullOrEmpty(token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating token");
                return false;
            }
        }

        public async Task LogoutAsync(string username)
        {
            try
            {
                // Implement logout logic (token blacklisting, session cleanup, etc.)
                _logger.LogInformation($"User {username} logged out");
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during logout");
            }
        }

        private string GenerateJwtToken(int userId, string username)
        {
            // Simplified token generation - implement proper JWT token creation
            var tokenData = $"{userId}:{username}:{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}";
            var tokenBytes = System.Text.Encoding.UTF8.GetBytes(tokenData);
            return Convert.ToBase64String(tokenBytes);
        }

        private async Task UpdateLastLoginAsync(int userId)
        {
            try
            {
                using var command = _context.Database.GetDbConnection().CreateCommand();
                command.CommandText = "sp_UpdateLastLogin";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@UserId", userId));

                await _context.Database.OpenConnectionAsync();
                await command.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating last login");
            }
            finally
            {
                await _context.Database.CloseConnectionAsync();
            }
        }
    }
}