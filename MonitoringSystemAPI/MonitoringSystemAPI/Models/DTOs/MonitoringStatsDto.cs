namespace MonitoringAPI.Models
{
    public class MonitoringStatsDto
    {
        public Dictionary<string, MonitoringStatItem> Stats { get; set; } = new();
        public DateTime RecordDate { get; set; }
        public bool IsBacklog { get; set; }
    }
}