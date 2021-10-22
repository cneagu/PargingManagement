namespace ParkingManagement.WebClient.Api.Framework
{
    public class Config : IConfig
    {
        public string AccessTokenKey { get; set; }
        public string RefreshTokenKey { get; set; }
        public int CloseAllocationsHour { get; set; }
    }
}
