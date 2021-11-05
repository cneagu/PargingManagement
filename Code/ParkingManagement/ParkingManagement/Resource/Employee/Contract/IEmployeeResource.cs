using System;
using System.Collections.Generic;

namespace ParkingManagement.Resource.Employee.Contract
{
    public interface IEmployeeResource
    {
        Employee[] SelectAll();
        Employee[] SelectEligibleByDepartments(IEnumerable<Guid> ids);
        Employee SelectByID(Guid id);
        Employee SelectByEmail(string email);
        Employee[] SelectByDepartmentID(Guid id);
        int CountByEmail(string email);
        void Insert(Employee employee);
        void Update(Employee employee);
    }
}
