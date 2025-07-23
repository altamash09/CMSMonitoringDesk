using System.ComponentModel.DataAnnotations;

namespace MonitoringSystemAPI.Models.Entities
{
    public class MonitoringStats
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        public int Count { get; set; } = 0;

        [StringLength(100)]
        public string Color { get; set; } = string.Empty;

        public bool IsMandatory { get; set; } = true;

        public DateTime Date { get; set; } = DateTime.UtcNow.Date;

        public bool IsBacklog { get; set; } = false;

        public DateTime LastUpdate { get; set; } = DateTime.UtcNow;
    }
}