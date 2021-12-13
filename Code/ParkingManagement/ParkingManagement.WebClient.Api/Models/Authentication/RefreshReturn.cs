namespace ParkingManagement.WebClient.Api.Models.Authentication
{
    public class RefreshReturn
    {
        public Token AccessToken { get; set; }
        public Token RefreshToken { get; set; }
    }
}
