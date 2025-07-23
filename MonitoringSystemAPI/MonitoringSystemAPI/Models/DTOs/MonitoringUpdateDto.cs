namespace MonitoringAPI.Models
{
    public class MonitoringUpdateDto
    {
        public string Status { get; set; }
        public int Count { get; set; }
        public DateTime Timestamp { get; set; }
    }
}