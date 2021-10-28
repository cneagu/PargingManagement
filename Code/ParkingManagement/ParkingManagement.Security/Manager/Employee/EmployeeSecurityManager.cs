using ParkingManagement.Infrastructure;
using ParkingManagement.Security.Contract.Employee;
using ParkingManagement.Security.Resource.Employee.Contract;
using System;

namespace ParkingManagement.Security.Manager.Employee
{
    public class EmployeeSecurityManager : IEmployeeSecurityManager
    {
        private readonly IEmployeeResource employeeResource;
        private readonly IEncryptionUtility encryptionUtility;

        public EmployeeSecurityManager(
            IEmployeeResource employeeResource,
            IEncryptionUtility encryptionUtility
            )
        {
            this.employeeResource = employeeResource;
            this.encryptionUtility = encryptionUtility;
        }

        public LoginReturn Login(string email, string password)
        {
            LoginReturn loginReturn = new LoginReturn();

            Resource.Employee.Contract.Employee employee = employeeResource.SelectByEmail(email);

            if (employee == null)
            {
                loginReturn.Status = LoginStatus.InvalidEmail;

                return loginReturn;
            }

            if (employee.Status == Status.Disabled)
            {
                loginReturn.Status = LoginStatus.AccountDisabled;

                return loginReturn;
            }

            if (!encryptionUtility.IsHashEqual(employee.Password, password))
            {
                loginReturn.Status = LoginStatus.InvalidPassword;

                return loginReturn;
            }

            loginReturn.Status = LoginStatus.Success;
            loginReturn.EmployeeID = employee.ID;

            return loginReturn;
        }

        public RegisterReturn Register(RegisterEmployee registerEmployee)
        {
            RegisterReturn registerReturn = new RegisterReturn();

            if (employeeResource.CountByEmail(registerEmployee.Email) > 0)
            {
                registerReturn.Status = RegisterStatus.InvalidEmail;

                return registerReturn;
            }

            string hashedPassword = encryptionUtility.ComputeHashWithSalt(registerEmployee.Password);

            Resource.Employee.Contract.Employee employee = new()
            {
                ID = Guid.NewGuid(),
                FirstName = registerEmployee.FirstName,
                LastName = registerEmployee.LastName,
                Email = registerEmployee.Email,
                Role = Role.Employee,
                Password = hashedPassword,
                Priority = 0,
                Status = Status.None,
                HasParkingSpot = false,
                DepartmentID = Guid.Empty
            };

            employeeResource.Insert(employee);

            registerReturn.Status = RegisterStatus.Success;
            registerReturn.EmployeeID = employee.ID;

            return registerReturn;
        }
    }
}
