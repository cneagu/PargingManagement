using System;
using System.Collections.Generic;

namespace ParkingManagement.Resource.ParkingSpot.Contract
{
    public interface IAvailableParkingSpotResource
    {
        AvailableParkingSpot[] SelectAll();
        AvailableParkingSpot[] SelectByEffectiveDate(DateTime date);
        AvailableParkingSpot SelectByID(Guid id);
        void Upsert(AvailableParkingSpot availableParkingSpot);
        List<AvailableParkingSpot> ListParkingSpotsBetweenInterval(Guid parkingSpotId, DateTime startDate, DateTime endDate);
        AvailableParkingSpot[] ListAvailableParkingSpot(Guid parkingSpotId, DateTime date);
        AvailableParkingSpot[] ListAvailableParkingSpotsByDepartment(Guid departmentID, DateTime date);
        void DeleteById(Guid id);
        void UpdateEmployeeParkingRequestFieldByID(Guid id, Guid? employeeParkingRequestId);
    }
}
