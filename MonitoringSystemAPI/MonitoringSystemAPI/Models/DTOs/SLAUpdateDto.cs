namespace MonitoringAPI.Models
{
    public class SLAUpdateDto
    {
        public int Hour { get; set; }
        public int Completed { get; set; }
        public decimal Percentage { get; set; }
        public DateTime Timestamp { get; set; }
    }
}