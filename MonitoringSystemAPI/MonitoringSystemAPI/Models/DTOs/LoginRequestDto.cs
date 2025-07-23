using System.ComponentModel.DataAnnotations;

namespace MonitoringAPI.Models
{
    // Request DTOs
    public class LoginRequestDto
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}