namespace MonitoringAPI.Models
{
    public class UserInfoDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string Permissions { get; set; } 
    }
}