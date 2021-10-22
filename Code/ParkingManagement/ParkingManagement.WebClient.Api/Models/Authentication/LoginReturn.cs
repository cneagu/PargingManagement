using System;

namespace ParkingManagement.WebClient.Api.Models.Authentication
{
    public class LoginReturn
    {
        public Guid EmployeeID { get; set; }
        public LoginStatus Status { get; set; }
    }
}
