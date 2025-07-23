namespace MonitoringAPI.Models
{
    public class UserStatusMessage
    {
        public int UserId { get; set; }
        public string UserType { get; set; }
        public string Status { get; set; }
        public string UserName { get; set; }
    }
}