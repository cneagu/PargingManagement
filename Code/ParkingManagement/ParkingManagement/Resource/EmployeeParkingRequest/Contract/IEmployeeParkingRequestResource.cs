using System;

namespace ParkingManagement.Resource.EmployeeParkingRequest.Contract
{
    public interface IEmployeeParkingRequestResource
    {
        EmployeeParkingRequest[] SelectAll();
        EmployeeParkingRequest[] SelectAvailableByEffectiveDate(DateTime date);
        EmployeeParkingRequest[] SelectByEffectiveDate(DateTime date);
        EmployeeParkingRequest SelectByID(Guid id);
        EmployeeParkingRequest[] SelectAlocatedByDepartmentAndEffectiveDate(Guid departmentID, DateTime date);
        void Insert(EmployeeParkingRequest employeeParkingRequest);
        void Upsert(EmployeeParkingRequest employeeParkingRequest);
        void UpdateAllocatedFieldByID(Guid id, bool allocated);
    }
}
