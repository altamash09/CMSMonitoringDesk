using System.ComponentModel.DataAnnotations;

namespace MonitoringSystemAPI.Models.Entities
{
    public class Agent
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "offline"; // online, idle, offline

        public int Completed { get; set; } = 0;

        public decimal EstimatedHours { get; set; } = 0;

        public decimal ActualHours { get; set; } = 0;

        [StringLength(20)]
        public string Rank { get; set; } = "Bronze"; // Diamond, Platinum, Gold, Silver, Bronze

        public int Efficiency { get; set; } = 0; // percentage

        public DateTime LastUpdate { get; set; } = DateTime.UtcNow;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}