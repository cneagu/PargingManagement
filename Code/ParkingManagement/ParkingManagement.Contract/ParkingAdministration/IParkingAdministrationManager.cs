using System;

namespace ParkingManagement.Contract.ParkingAdministration
{
    public interface IParkingAdministrationManager
    {
        DateInputStatus AddAvailableParkingSpot(Guid userId, DateInput dateInput);
        AvailableParkingSpot[] ListAvailableParkingSpot(Guid userId, DateTime effectiveDate);
        void DeleteAvailableParkingSpot(Guid parkingSpotId);
        Department[] ListDepartments();
    }
}
