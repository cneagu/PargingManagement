using ParkingManagement.Contract.Employee;
using ParkingManagement.Infrastructure;
using ParkingManagement.Resource.Employee.Contract;
using System;

namespace ParkingManagement.Manager
{
    public class EmployeeAdministrationManager : IEmployeeAdministrationManager
    {
        private readonly IEmployeeResource employeeResource;

        public EmployeeAdministrationManager(IEmployeeResource employeeResource)
        {
            this.employeeResource = employeeResource;
        }

        public Contract.Employee.Employee GetEmployeeByEmail(string email)
        {
            Resource.Employee.Contract.Employee employee = employeeResource.SelectByEmail(email);
            return employee.DeepCopy<Contract.Employee.Employee>();
        }

        public void AddEmployee(Contract.Employee.Employee employee)
        {
            employeeResource.Insert(employee.DeepCopy<Resource.Employee.Contract.Employee>());
        }

        public int CountEmployeesByEmail(string email)
        {
            return employeeResource.CountByEmail(email);
        }

        public Contract.Employee.Employee DetailEmployeeByID(Guid employeeId)
        {
            Resource.Employee.Contract.Employee employee = employeeResource.SelectByID(employeeId);
            return employee.DeepCopy<Contract.Employee.Employee>();
        }
    }
}
