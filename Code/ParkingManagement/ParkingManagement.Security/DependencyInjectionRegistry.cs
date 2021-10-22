using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ParkingManagement.Infrastructure;
using ParkingManagement.Security.Contract.Employee;
using ParkingManagement.Security.Manager.Employee;
using ParkingManagement.Security.Resource.Employee;
using ParkingManagement.Security.Resource.Employee.Contract;

namespace ParkingManagement.Security
{
    public static class DependencyInjectionRegistry
    {
        public static IServiceCollection AddSecurityServices(this IServiceCollection services, IConfiguration configuration)
        {
            Config config1 = new();
            configuration.GetSection("DBConnections").Bind(config1);

            return services.AddSingleton<IEmployeeSecurityManager, EmployeeSecurityManager>()
                    .AddSingleton<IEmployeeResource>(x => new EmployeeResource(config1))
                    .AddSingleton<IEncryptionUtility, EncryptionUtility>();
        }
    }
}
