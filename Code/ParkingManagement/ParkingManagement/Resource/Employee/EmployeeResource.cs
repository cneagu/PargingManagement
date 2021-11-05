using ParkingManagement.Infrastructure;
using ParkingManagement.Resource.Employee.Contract;
using System;
using System.Collections.Generic;

namespace ParkingManagement.Resource.Employee
{
    public class EmployeeResource : IEmployeeResource
    {
        private readonly IConfig config;

        public EmployeeResource(IConfig config)
        {
            this.config = config;
        }

        public Contract.Employee[] SelectAll()
        {
            return SqlDataAccess.Read<Contract.Employee>("SELECT * FROM Employee", config.ConnectionString).ToArray();
        }

        public Contract.Employee[] SelectEligibleByDepartments(IEnumerable<Guid> ids)
        {
            return SqlDataAccess.Read<Contract.Employee>(

                string.Format("SELECT * FROM Employee WHERE Status = {0} AND HasParkingSpot = 0 AND DepartmentID IN ({1}) ",
                    ((int)Status.Active).ToString().ToSqlValue(),
                    ids.ToSqlInClause()), config.ConnectionString).ToArray();
        }

        public Contract.Employee SelectByID(Guid id)
        {
            List<Contract.Employee> employee = SqlDataAccess.Read<Contract.Employee>(string.Format("SELECT * FROM Employee WHERE ID = {0}", id.ToSqlValue()), config.ConnectionString);
            return employee.Count > 0 ? employee[0] : null;
        }

        public Contract.Employee[] SelectByDepartmentID(Guid id)
        {
            return SqlDataAccess.Read<Contract.Employee>(string.Format("SELECT * FROM Employee WHERE DepartmentID = {0}", id.ToSqlValue()), config.ConnectionString).ToArray();
        }

        public Contract.Employee SelectByEmail(string email)
        {
            List<Contract.Employee> employee = SqlDataAccess.Read<Contract.Employee>(string.Format("SELECT * FROM Employee WHERE Email = {0}", email.ToSqlValue()), config.ConnectionString);
            return employee.Count > 0 ? employee[0] : null;
        }
        public int CountByEmail(string email)
        {
            return (int)SqlDataAccess.ExecuteScalar(string.Format("SELECT Count(*) FROM Employee WHERE Email = {0}", email.ToSqlValue()), config.ConnectionString);
        }

        public void Insert(Contract.Employee employee)
        {
            SqlDataAccess.Insert(employee, "Employee", config.ConnectionString);
        }

        public void Update(Contract.Employee employee)
        {
            SqlDataAccess.Update(employee, "Employee", SqlDataAccess.CreateWhereClause("ID", employee.ID.ToSqlValue()), config.ConnectionString);
        }
    }
}
