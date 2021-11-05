using System;

namespace ParkingManagement.Contract.Employee
{
    public interface IEmployeeAdministrationManager
    {
        Employee GetEmployeeByEmail(string email);
        void AddEmployee(Employee employee);
        int CountEmployeesByEmail(string email);
        Employee DetailEmployeeByID(Guid employeeId);
    }
}
