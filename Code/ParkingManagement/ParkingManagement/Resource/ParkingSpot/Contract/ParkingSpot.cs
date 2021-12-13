using System;

namespace ParkingManagement.Resource.ParkingSpot.Contract
{
    public class ParkingSpot
    {
        public Guid ID { get; set; }
        public int Number { get; set; }
        public Type Type { get; set; }
        public Guid EmployeeID { get; set; }
        public Guid DepartmentID { get; set; }
    }

    public enum Type
    {
        None = 0,
        Single = 1,
        Double = 2
    }
}
