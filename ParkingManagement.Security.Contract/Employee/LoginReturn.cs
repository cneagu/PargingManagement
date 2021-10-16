using System;

namespace ParkingManagement.Security.Contract.Employee
{
    public class LoginReturn
    {
        public Guid EmployeeID { get; set; }
        public LoginStatus Status { get; set; }
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
