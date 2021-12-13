using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ParkingManagement.Contract.Employee;
using ParkingManagement.Contract.ParkingAdministration;
using ParkingManagement.Contract.ParkingAllocation;
using ParkingManagement.Infrastructure;
using ParkingManagement.Manager;
using ParkingManagement.Resource.Department;
using ParkingManagement.Resource.Department.Contract;
using ParkingManagement.Resource.Employee;
using ParkingManagement.Resource.Employee.Contract;
using ParkingManagement.Resource.EmployeeParkingRequest;
using ParkingManagement.Resource.EmployeeParkingRequest.Contract;
using ParkingManagement.Resource.ParkingSpot;
using ParkingManagement.Resource.ParkingSpot.Contract;

namespace ParkingManagement
{
    public static class DependencyInjectionRegistry
    {
        public static IServiceCollection AddBusinessServices(this IServiceCollection services, IConfiguration configuration)
        {
            Config config = new();
            configuration.GetSection("DBConnections").Bind(config);


            services.AddSingleton<IEmployeeAdministrationManager, EmployeeAdministrationManager>()
                    .AddSingleton<IParkingAdministrationManager, ParkingAdministrationManager>()
                    .AddSingleton<IParkingAllocationManager, ParkingAllocationManager>();


            services.AddSingleton<IEmployeeResource>(_ => new EmployeeResource(config))
                    .AddSingleton<IAvailableParkingSpotResource, AvailableParkingSpotResource>()
                    .AddSingleton<IEmployeeParkingRequestResource, EmployeeParkingRequestResource>()
                    .AddSingleton<IParkingSpotResource>(_ => new ParkingSpotResource(config))
                    .AddSingleton<IDepartmentResource>(_ => new DepartmentResource(config))
                    .AddSingleton<IDateTimeProvider, DateTimeProvider>()
                    .AddTransient<IMongoDataAccess>(_ => new MongoDataAccess(config));

            return services;
        }
    }
}
