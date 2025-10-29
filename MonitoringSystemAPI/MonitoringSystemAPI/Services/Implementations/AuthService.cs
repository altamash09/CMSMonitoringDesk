using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
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
                        var role = reader.IsDBNull("Role") ? "user" : reader.GetString("Role");
                        var permissionsJson = reader.IsDBNull("Permissions") ? "[]" : reader.GetString("Permissions");

                        // Generate JWT token with permissions
                        var token = GenerateJwtToken(userId, username, email, role, permissionsJson);

                        // Update last login
                        await reader.CloseAsync();
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
                                Email = email,
                                Role = role,
                                Permissions = permissionsJson
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
                var jwtSettings = _configuration.GetSection("JWT");
                var secretKey = jwtSettings["SecretKey"] ?? "YourSecretKeyHere_MakeItLongAndSecure_AtLeast32Characters";
                var key = Encoding.ASCII.GetBytes(secretKey);

                var tokenHandler = new JwtSecurityTokenHandler();
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings["Issuer"] ?? "MonitoringAPI",
                    ValidateAudience = true,
                    ValidAudience = jwtSettings["Audience"] ?? "MonitoringClients",
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };

                var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);

                return validatedToken != null;
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

        private string GenerateJwtToken(int userId, string username, string email, string role, string permissionsJson)
        {
            var jwtSettings = _configuration.GetSection("JWT");
            var secretKey = jwtSettings["SecretKey"] ?? "YourSecretKeyHere_MakeItLongAndSecure_AtLeast32Characters";
            var key = Encoding.ASCII.GetBytes(secretKey);

            var tokenHandler = new JwtSecurityTokenHandler();

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Email, email ?? ""),
                new Claim(ClaimTypes.Role, role),
                new Claim("userId", userId.ToString()),
                new Claim("username", username),
                new Claim("permissions", permissionsJson)
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(int.Parse(jwtSettings["ExpirationMinutes"] ?? "480")),
                Issuer = jwtSettings["Issuer"] ?? "MonitoringAPI",
                Audience = jwtSettings["Audience"] ?? "MonitoringClients",
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private async Task UpdateLastLoginAsync(int userId)
        {
            try
            {
                using var command = _context.Database.GetDbConnection().CreateCommand();
                command.CommandText = "sp_UpdateLastLogin";
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add(new SqlParameter("@UserId", userId));

                if (_context.Database.GetDbConnection().State != ConnectionState.Open)
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