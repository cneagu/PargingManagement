using System;

namespace ParkingManagement.Contract.ParkingAllocation
{
    public class Notification
    {
        public string EmployeeName { get; set; }
        public string Email { get; set; }
        public DateTime Date { get; set; }
        public Guid LinkId { get; set; }
    }
}
