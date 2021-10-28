using ParkingManagement.Infrastructure;
using ParkingManagement.Security.Resource.Employee.Contract;
using System.Collections.Generic;

namespace ParkingManagement.Security.Resource.Employee
{
    public class EmployeeResource : IEmployeeResource
    {
        private readonly IConfig config;

        public EmployeeResource(IConfig config)
        {
            this.config = config;
        }

        public Contract.Employee SelectByEmail(string email)
        {
            List<Contract.Employee> employee = SqlDataAccess.Read<Contract.Employee>(string.Format("SELECT * FROM Employee WHERE Email = {0}", email.ToSqlValue()), config.ConnectionString);
            return employee.Count > 0 ? employee[0] : null;
        }

        public void Insert(Contract.Employee employee)
        {
            SqlDataAccess.Insert(employee, "Employee", config.ConnectionString);
        }

        public int CountByEmail(string email)
        {
            return (int)SqlDataAccess.ExecuteScalar(string.Format("SELECT Count(*) FROM Employee WHERE Email = {0}", email.ToSqlValue()), config.ConnectionString);
        }
    }
}
