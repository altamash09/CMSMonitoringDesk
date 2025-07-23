namespace MonitoringSystemAPI.Models.Entities
{
    public class HourlyData
    {
        public int Id { get; set; }

        public int Hour { get; set; } // 0-23

        public int Completed { get; set; } = 0;

        public int Percentage { get; set; } = 0;

        public DateTime Date { get; set; } = DateTime.UtcNow.Date;

        public DateTime LastUpdate { get; set; } = DateTime.UtcNow;
    }
}