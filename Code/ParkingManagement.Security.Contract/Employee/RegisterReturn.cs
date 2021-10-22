using System;

namespace ParkingManagement.Security.Contract.Employee
{
    public class RegisterReturn
    {
        public Guid EmployeeID { get; set; }
        public RegisterStatus Status { get; set; }
    }

    public enum RegisterStatus
    {
        None = 0,
        Success = 1,
        InvalidEmail = 2
    }
}
