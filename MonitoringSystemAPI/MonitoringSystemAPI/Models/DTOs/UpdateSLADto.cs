namespace MonitoringAPI.Models
{
    public class UpdateSLADto
    {
        public int Hour { get; set; }
        public int Completed { get; set; }
        public decimal Percentage { get; set; }
        public DateTime Date { get; set; }
    }
}