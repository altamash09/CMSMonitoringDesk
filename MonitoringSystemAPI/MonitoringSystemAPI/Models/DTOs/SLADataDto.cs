namespace MonitoringAPI.Models
{
    public class SLADataDto
    {
        public int Hour { get; set; }
        public int Completed { get; set; }
        public decimal Percentage { get; set; }
    }
}