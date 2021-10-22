using ParkingManagement.Infrastructure;
using ParkingManagement.Security.Resource.Employee.Contract;

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
            Contract.Employee employee = SqlDataAccess.Read<Contract.Employee>(string.Format("SELECT * FROM Employee WHERE Email = {0}", email.ToSqlValue()), config.ConnectionString)[0];

            return employee;
        }

        public void Insert(Contract.Employee employee)
        {
            //employeeadministrationmanager.addemployee(employee.deepcopyto<parkingmanagement.contract.employee.employee>());
            throw new System.NotImplementedException();
        }

        public int CountByEmail(string email)
        {
            return (int)SqlDataAccess.ExecuteScalar(string.Format("SELECT Count(*) FROM Employee WHERE Email = {0}", email.ToSqlValue()), config.ConnectionString);
        }
    }
}
