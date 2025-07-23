namespace MonitoringAPI.Models
{
    public class LoginResponseDto
    {
        public bool Success { get; set; }
        public string Token { get; set; }
        public string Message { get; set; }
        public UserInfoDto User { get; set; }
    }
}