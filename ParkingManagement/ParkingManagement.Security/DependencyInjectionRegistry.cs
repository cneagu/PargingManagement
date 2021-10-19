using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ParkingManagement.Infrastructure;
using ParkingManagement.Security.Contract.Employee;
using ParkingManagement.Security.Manager.Employee;
using ParkingManagement.Security.Resource.Employee;
using System.Configuration;

namespace ParkingManagement.Security
{
    public static class DependencyInjectionRegistry
    {
        public static IServiceCollection AddSecurityServices(this IServiceCollection services, IConfiguration Configuration)
        {
            Security.Config config1 = new();
            Configuration.GetSection("DBConnections").Bind(config1);
            services.AddSingleton<IEmployeeSecurityManager>(x => new EmployeeSecurityManager(
                new EmployeeResource(config1),
                new EncryptionUtility()
                ));

            return services;
        }
    }
}
