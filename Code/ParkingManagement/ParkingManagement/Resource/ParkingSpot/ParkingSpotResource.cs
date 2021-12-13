using ParkingManagement.Infrastructure;
using ParkingManagement.Resource.ParkingSpot.Contract;
using System;
using System.Collections.Generic;

namespace ParkingManagement.Resource.ParkingSpot
{
    public class ParkingSpotResource : IParkingSpotResource
    {
        private readonly IConfig config;

        public ParkingSpotResource(IConfig config)
        {
            this.config = config;
        }

        public Contract.ParkingSpot[] SelectAll()
        {
            return SqlDataAccess.Read<Contract.ParkingSpot>("SELECT * FROM ParkingSpot", config.ConnectionString).ToArray();
        }

        public Contract.ParkingSpot[] SelectAllByIDs(IEnumerable<Guid> ids)
        {
            return SqlDataAccess.Read<Contract.ParkingSpot>(
                string.Format("SELECT * FROM ParkingSpot WHERE ID IN ({0})", ids.ToSqlInClause()),
                config.ConnectionString
            ).ToArray();
        }

        public Contract.ParkingSpot SelectByID(Guid id)
        {
            List<Contract.ParkingSpot> parkingSpot = SqlDataAccess.Read<Contract.ParkingSpot>(string.Format("SELECT * FROM ParkingSpot WHERE ID IN ({0})", id.ToSqlValue()), config.ConnectionString);
            return parkingSpot.Count > 0 ? parkingSpot[0] : null;
        }

        public Contract.ParkingSpot SelectByEmployeeID(Guid id)
        {
            List<Contract.ParkingSpot> parkingSpot = SqlDataAccess.Read<Contract.ParkingSpot>(string.Format("SELECT * FROM ParkingSpot WHERE EmployeeID = {0}", id.ToSqlValue()), config.ConnectionString);
            return parkingSpot.Count > 0 ? parkingSpot[0] : null;
        }

        public Contract.ParkingSpot[] SelectByDepartmentID(Guid id)
        {
            return SqlDataAccess.Read<Contract.ParkingSpot>(string.Format("SELECT * FROM ParkingSpot WHERE DepartmentID = {0}", id.ToSqlValue()), config.ConnectionString).ToArray();
        }

        public void Insert(Contract.ParkingSpot parkingSpot)
        {
            SqlDataAccess.Insert(parkingSpot, "ParkingSpot", config.ConnectionString);
        }

        public void Update(Contract.ParkingSpot parkingSpot)
        {
            SqlDataAccess.Update(parkingSpot, "ParkingSpot", SqlDataAccess.CreateWhereClause("ID", parkingSpot.ID.ToSqlValue()), config.ConnectionString);
        }

        public int CountByID(Guid employeeId)
        {
            return (int)SqlDataAccess.ExecuteScalar(string.Format("SELECT Count(*) FROM ParkingSpot WHERE EmployeeID = {0}", employeeId.ToSqlValue()), config.ConnectionString);
        }
    }
}
