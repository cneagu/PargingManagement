using System;

namespace ParkingManagement.WebClient.Api.Models.Authentication
{
    public class Token
    {
        public DateTime Expires { get; set; }
        public string Value { get; set; }
    }
}
