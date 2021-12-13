using System;

namespace ParkingManagement.Contract.ParkingAdministration
{
    public class AvailableParkingSpot
    {
        public Guid ID { get; set; }
        public Guid ParkingSpotID { get; set; }
        public Guid DepartmentID { get; set; }
        public DateTime EffectiveDate { get; set; }
        public Guid? EmployeeParkingRequestID { get; set; }
    }
}
