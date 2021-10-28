using System;

namespace ParkingManagement.Security.Resource.Employee.Contract
{
    public class Employee
    {
        public Guid ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public Role Role { get; set; }
        public string Password { get; set; }
        public decimal Priority { get; set; }
        public Status Status { get; set; }
        public bool HasParkingSpot { get; set; }
        public Guid DepartmentID { get; set; }
    }

    public enum Role
    {
        None = 0,
        Admin = 1,
        Employee = 2
    }

    public enum Status
    {
        None = 0,
        Active = 1,
        Disabled = 2
    }
}
