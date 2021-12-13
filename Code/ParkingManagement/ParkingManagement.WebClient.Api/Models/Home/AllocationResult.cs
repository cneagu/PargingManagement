using System;

namespace ParkingManagement.WebClient.Api.Models.Home
{
    public class AllocationResult
    {
        public Guid ID { get; set; }
        public string EmployeeName { get; set; }
        public string Email { get; set; }
        public int ParkingSpotNumber { get; set; }
        public DateTime Date { get; set; }
    }
}
