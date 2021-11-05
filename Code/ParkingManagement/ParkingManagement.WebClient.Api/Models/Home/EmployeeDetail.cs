using System;

namespace ParkingManagement.WebClient.Api.Models.Home
{
    public class EmployeeDetail
    {
        public bool IsParkingOwner { get; set; }

        public bool IsAccountActivated { get; set; }

        public Guid DepartmentId { get; set; }
    }
}
