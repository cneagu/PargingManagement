using ParkingManagement.Infrastructure;
using ParkingManagement.Resource.Department.Contract;

namespace ParkingManagement.Resource.Department
{
    public class DepartmentResource : IDepartmentResource
    {
        private readonly IConfig config;

        public DepartmentResource(IConfig config)
        {
            this.config = config;
        }

        public Contract.Department[] SelectAll()
        {
            return SqlDataAccess.Read<Contract.Department>("SELECT * FROM Department", config.ConnectionString).ToArray();
        }
    }
}
