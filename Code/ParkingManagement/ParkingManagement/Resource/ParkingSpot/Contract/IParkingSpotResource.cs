using System;
using System.Collections.Generic;

namespace ParkingManagement.Resource.ParkingSpot.Contract
{
    public interface IParkingSpotResource
    {
        ParkingSpot[] SelectAll();
        ParkingSpot[] SelectAllByIDs(IEnumerable<Guid> ids);
        ParkingSpot SelectByID(Guid id);
        ParkingSpot SelectByEmployeeID(Guid id);
        ParkingSpot[] SelectByDepartmentID(Guid id);
        void Insert(ParkingSpot parkingSpot);
        void Update(ParkingSpot parkingSpot);
        int CountByID(Guid employeeId);
    }
}
