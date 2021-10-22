namespace ParkingManagement.WebClient.Api.Models.Authentication
{
    public class LoginResult
    {
        public LoginStatus Status { get; set; }
        public Token AccessToken { get; set; }
        public Token RefreshToken { get; set; }
    }

    public enum LoginStatus
    {
        None = 0,
        Success = 1,
        InvalidEmail = 2,
        AccountDisabled = 3,
        InvalidPassword = 4
    }
}
