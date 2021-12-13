using System;

namespace ParkingManagement.Resource.EmployeeParkingRequest.Contract
{
    public class EmployeeParkingRequest
    {
        public Guid ID { get; set; }
        public Guid EmployeeID { get; set; }
        public PreferredType PreferredType { get; set; }
        public DateTime EffectiveDate { get; set; }
        public bool Accepted { get; set; }
        public bool Allocated { get; set; }
        public double Priority { get; set; }
        public Guid DepartmentID { get; set; }
    }

    public enum PreferredType
    {
        None = 0,
        Single = 1,
        Any = 2
    }
}
