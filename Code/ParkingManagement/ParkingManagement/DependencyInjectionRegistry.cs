using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ParkingManagement.Contract.Employee;
using ParkingManagement.Manager;
using ParkingManagement.Resource.Employee;
using ParkingManagement.Resource.Employee.Contract;

namespace ParkingManagement
{
    public static class DependencyInjectionRegistry
    {
        public static IServiceCollection AddBusinessServices(this IServiceCollection services, IConfiguration configuration)
        {
            Config config = new();
            configuration.GetSection("DBConnections").Bind(config);


            services.AddSingleton<IEmployeeAdministrationManager, EmployeeAdministrationManager>()
                    .AddSingleton<IEmployeeResource>(x => new EmployeeResource(config));

            return services;
        }
    }
}
