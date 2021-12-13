namespace ParkingManagement.WebClient.Api.Models.Authentication
{
    public enum ForgotPasswordReturn
    {
        None = 0,
        Success = 1,
        InvalidEmail = 2,
        AccountDisabled = 3
    }
}
