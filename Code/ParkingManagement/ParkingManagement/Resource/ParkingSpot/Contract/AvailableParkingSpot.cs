using MongoDB.Bson.Serialization.Attributes;
using System;

namespace ParkingManagement.Resource.ParkingSpot.Contract
{
    public class AvailableParkingSpot
    {
        [BsonId]
        public Guid ID { get; set; }
        public Guid ParkingSpotID { get; set; }
        public Guid DepartmentID { get; set; }
        public DateTime EffectiveDate { get; set; }
        public Guid? EmployeeParkingRequestID { get; set; }
    }
}
