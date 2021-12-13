using System;

namespace ParkingManagement.Contract.ParkingAllocation
{
    public interface IParkingAllocationManager
    {
        ParkingRequestStatus ResolveParkingRequest(Guid requestID, PreferredType type);
        AllocationResult[] AllocateParkingSpots();
        Notification[] NotifyAvailableParkingSpots();
        Notification[] NotifyAvailableParkingSpotsDynamic();
        AllocationResult[] ListAllocationResults(Guid employeeId, DateTime effectiveDate);
    }
}
