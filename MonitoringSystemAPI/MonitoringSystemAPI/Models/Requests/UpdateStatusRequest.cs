using System.ComponentModel.DataAnnotations;

namespace MonitoringSystemAPI.Models.Requests
{
    public class UpdateStatusRequest
    {
        [Required]
        [StringLength(20)]
        public string Status { get; set; } = string.Empty;
    }
}