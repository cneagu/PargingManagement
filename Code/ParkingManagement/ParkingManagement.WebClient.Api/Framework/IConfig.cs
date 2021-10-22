namespace ParkingManagement.WebClient.Api.Framework
{
    public interface IConfig
    {
        string AccessTokenKey { get; set; }
        string RefreshTokenKey { get; set; }
        int CloseAllocationsHour { get; set; }
    }
}
