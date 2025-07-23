using System.ComponentModel.DataAnnotations;

namespace MonitoringSystemAPI.Models.Requests
{
    public class UpdateUserRequest
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(200)]
        public string Email { get; set; } = string.Empty;

        [Phone]
        [StringLength(20)]
        public string? Phone { get; set; }

        [Required]
        [StringLength(50)]
        public string Role { get; set; } = string.Empty;

        [StringLength(100)]
        public string? Department { get; set; }

        public List<string> Permissions { get; set; } = new();
    }
}