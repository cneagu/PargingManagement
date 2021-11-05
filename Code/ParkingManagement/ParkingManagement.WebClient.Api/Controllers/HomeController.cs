using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ParkingManagement.Contract.Employee;
using ParkingManagement.Infrastructure;
using ParkingManagement.WebClient.Api.Models.Home;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ParkingManagement.WebClient.Api.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class HomeController : ControllerBase
    {
        private readonly int closeAllocationsHour;
        //public IParkingAdministrationManager parkingAdministrationManager { get; set; }
        public IDateTimeProvider dateTimeProvider;
        public IEmployeeAdministrationManager employeeAdministrationManager;
        //public IParkingAllocationManager parkingAllocationManager { get; set; }

        public HomeController(Framework.IConfig config,
            //IParkingAdministrationManager parkingAdministrationManager,
            IDateTimeProvider dateTimeProvider,
            IEmployeeAdministrationManager employeeAdministrationManager)
            //IParkingAllocationManager parkingAllocationManager)
        {
            closeAllocationsHour = config.CloseAllocationsHour;
            //this.parkingAdministrationManager = parkingAdministrationManager;
            this.dateTimeProvider = dateTimeProvider;
            this.employeeAdministrationManager = employeeAdministrationManager;
            //this.parkingAllocationManager = parkingAllocationManager;
        }

        [HttpGet]
        public DateTime GetMinDateOfParkingSelection()
        {
            return GetMinDateOfSelection();
        }

        [HttpGet]
        public async Task<IActionResult> GetEmployeeDetails()
        {
            ClaimsIdentity identity = HttpContext.User.Identity as ClaimsIdentity;
            Guid employeeId = new(identity.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value);

            Employee employee = await Task.Run(() =>
            {
                return employeeAdministrationManager.DetailEmployeeByID(employeeId);
            });

            EmployeeDetail emplyeeDetails = new EmployeeDetail
            {
                IsParkingOwner = employee.HasParkingSpot,
                IsAccountActivated = employee.Status != Status.None,
                DepartmentId = employee.DepartmentID
            };
            return Ok(emplyeeDetails);
        }

        //[HttpPost]
        //public async Task<Models.Home.DateInputStatus> AddAvailableParkingSpot(Models.Home.DateInput dateInput)
        //{
        //    ClaimsIdentity identity = HttpContext.Current.User.Identity as ClaimsIdentity;
        //    Guid employeeId = new Guid(identity.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value);

        //    Models.Home.DateInputStatus status = await Task.Run(() =>
        //    {
        //        return (Models.Home.DateInputStatus)parkingAdministrationManager.AddAvailableParkingSpot(employeeId, dateInput.DeepCopyTo<Contract.ParkingAdministration.DateInput>());
        //    });

        //    return status;
        //}

        //[HttpGet]
        //public async Task<Models.Home.AvailableParkingSpot[]> GetFutureAvailableParkingSpot()
        //{
        //    ClaimsIdentity identity = HttpContext.Current.User.Identity as ClaimsIdentity;
        //    Guid employeeId = new Guid(identity.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value);

        //    Contract.ParkingAdministration.AvailableParkingSpot[] parkingSpots = await Task.Run(() =>
        //    {
        //        return parkingAdministrationManager.ListAvailableParkingSpot(employeeId, GetMinDateOfSelection());
        //    });

        //    return parkingSpots.Select(x =>
        //    {
        //        return new Models.Home.AvailableParkingSpot
        //        {
        //            ID = x.ID.ToString(),
        //            Date = x.EffectiveDate.ToString("dd/MM/yyyy")
        //        };
        //    }).OrderByDescending(x => x.Date).ToArray();
        //}

        //[HttpGet("{id}")]
        //public async Task<DashboardData> GetDashboardData(Guid departmentId)
        //{
        //    DashboardData dashboardData = new DashboardData
        //    {
        //        Departments = await Task.Run(() =>
        //        {
        //            return parkingAdministrationManager.ListDepartments().DeepCopyTo<Models.Home.Department[]>();
        //        }),
        //        AllocationResults = await Task.Run(() =>
        //        {
        //            return parkingAllocationManager.ListAllocationResults(departmentId, dateTimeProvider.DateUtc()).DeepCopyTo<Models.Home.AllocationResult[]>();
        //        }),
        //    };

        //    return dashboardData;
        //}

        //[HttpGet("{id}")]
        //public async Task<Models.Home.AllocationResult[]> GetAllocationResult([FromUri] DateTime date, Guid departmentId)
        //{
        //    Models.Home.AllocationResult[] allocationResult = await Task.Run(() =>
        //    {
        //        return parkingAllocationManager.ListAllocationResults(departmentId, dateTimeProvider.DateUtc(date)).DeepCopyTo<Models.Home.AllocationResult[]>();
        //    });

        //    return allocationResult;
        //}

        //[HttpDelete]
        //public void DeleteAvailableParkingSpot(Guid id)
        //{
        //    parkingAdministrationManager.DeleteAvailableParkingSpot(id);
        //}

        private DateTime GetMinDateOfSelection()
        {
            DateTime refDate = dateTimeProvider.DateTimeUtc();

            if (refDate.Hour < closeAllocationsHour)
            {
                return refDate.Date;
            }
            return refDate.AddDays(1).Date;
        }
    }
}
