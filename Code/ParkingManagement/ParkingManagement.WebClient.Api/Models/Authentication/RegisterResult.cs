namespace ParkingManagement.WebClient.Api.Models.Authentication
{
    public class RegisterResult
    {
        public RegisterStatus Status { get; set; }
        public Token AccessToken { get; set; }
        public Token RefreshToken { get; set; }
    }

    public enum RegisterStatus
    {
        None = 0,
        Success = 1,
        InvalidEmail = 2
    }
}
