namespace MonitoringAPI.Models
{
    public class UserStatsDto
    {
        public int Total { get; set; }
        public int Online { get; set; }
        public int Idle { get; set; }
        public int Offline { get; set; }
    }
}