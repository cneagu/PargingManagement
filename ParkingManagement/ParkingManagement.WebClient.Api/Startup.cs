using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ParkingManagement.Infrastructure;
using ParkingManagement.Security.Contract.Employee;
using ParkingManagement.Security.Manager.Employee;
using ParkingManagement.Security.Resource.Employee;
using ParkingManagement.Security.Resource.Employee.Contract;
using System;
using System.Text;

namespace ParkingManagement.WebClient.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            Framework.IConfig config = new Framework.Config();
            Configuration.GetSection("ApiConfig").Bind(config);
            services.AddSingleton<Framework.IConfig>(x => (Framework.Config)config);

            Security.Config config1 = new();
            Configuration.GetSection("DBConnections").Bind(config1);
            services.AddSingleton<IEmployeeSecurityManager>(x => new EmployeeSecurityManager(
                new EmployeeResource(config1),
                new EncryptionUtility()
                ));

            //services.AddSingleton<IEmployeeResource, EmployeeResource>();
            //services.AddSingleton<IEncryptionUtility, EncryptionUtility>();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateLifetime = true,
                        ValidateIssuer = true,
                        ValidIssuer = "Parking.Authentication.Bearer",
                        ValidateAudience = true,
                        ValidAudience = "Parking.Authentication.Bearer",
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(config.AccessTokenKey)),
                        ClockSkew = TimeSpan.FromMinutes(0)

                    };
                });

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "ParkingManagement.WebClient.Api", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ParkingManagement.WebClient.Api v1"));
            }

            app.UseRouting();

            app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
