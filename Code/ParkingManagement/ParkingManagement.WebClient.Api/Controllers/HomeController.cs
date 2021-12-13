using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ParkingManagement.Contract.Employee;
using ParkingManagement.Contract.ParkingAdministration;
using ParkingManagement.Contract.ParkingAllocation;
using ParkingManagement.Infrastructure;
using ParkingManagement.WebClient.Api.Models.Home;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AllocationResult = ParkingManagement.WebClient.Api.Models.Home.AllocationResult;
using AvailableParkingSpot = ParkingManagement.WebClient.Api.Models.Home.AvailableParkingSpot;
using DateInput = ParkingManagement.WebClient.Api.Models.Home.DateInput;
using DateInputStatus = ParkingManagement.WebClient.Api.Models.Home.DateInputStatus;
using Department = ParkingManagement.WebClient.Api.Models.Home.Department;

namespace ParkingManagement.WebClient.Api.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    //[Authorize]
    public class HomeController : ControllerBase
    {
        private readonly int closeAllocationsHour;
        public IParkingAdministrationManager parkingAdministrationManager;
        public IDateTimeProvider dateTimeProvider;
        public IEmployeeAdministrationManager employeeAdministrationManager;
        public IParkingAllocationManager parkingAllocationManager;

        public HomeController(Framework.IConfig config,
            IParkingAdministrationManager parkingAdministrationManager,
            IDateTimeProvider dateTimeProvider,
            IEmployeeAdministrationManager employeeAdministrationManager,
            IParkingAllocationManager parkingAllocationManager)
        {
            closeAllocationsHour = config.CloseAllocationsHour;
            this.parkingAdministrationManager = parkingAdministrationManager;
            this.dateTimeProvider = dateTimeProvider;
            this.employeeAdministrationManager = employeeAdministrationManager;
            this.parkingAllocationManager = parkingAllocationManager;
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
            if (!identity.Claims.Any())
                return BadRequest();

            Guid employeeId = new(identity.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value);

            Employee employee = await Task.Run(() =>
            {
                return employeeAdministrationManager.DetailEmployeeByID(employeeId);
            });

            EmployeeDetail emplyeeDetails = new()
            {
                IsParkingOwner = employee.HasParkingSpot,
                IsAccountActivated = employee.Status != Status.None,
                DepartmentId = employee.DepartmentID
            };
            return Ok(emplyeeDetails);
        }

        [HttpPost]
        public async Task<DateInputStatus> AddAvailableParkingSpot(DateInput dateInput)
        {
            ClaimsIdentity identity = HttpContext.User.Identity as ClaimsIdentity;
            if (!identity.Claims.Any())
                return DateInputStatus.None;

            Guid employeeId = new(identity.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value);

            DateInputStatus status = await Task.Run(() =>
            {
                return (DateInputStatus)parkingAdministrationManager.AddAvailableParkingSpot(employeeId, dateInput.DeepCopy<Contract.ParkingAdministration.DateInput>());
            });

            return status;
        }

        [HttpGet]
        public async Task<AvailableParkingSpot[]> GetFutureAvailableParkingSpot()
        {
            //ClaimsIdentity identity = HttpContext.User.Identity as ClaimsIdentity;
            //if (!identity.Claims.Any())
            //    return null;

            //Guid employeeId = new(identity.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value);
            Guid employeeId = new("5BCD305C-B050-425C-A8F5-9FC3E09AA6C6");

            Contract.ParkingAdministration.AvailableParkingSpot[] parkingSpots = await Task.Run(() =>
            {
                return parkingAdministrationManager.ListAvailableParkingSpot(employeeId, GetMinDateOfSelection());
            });

            return parkingSpots.Select(x =>
            {
                return new AvailableParkingSpot
                {
                    ID = x.ID.ToString(),
                    Date = x.EffectiveDate.ToString("dd/MM/yyyy")
                };
            }).OrderByDescending(x => x.Date).ToArray();
        }

        [HttpGet("{departmentId}")]
        public async Task<DashboardData> GetDashboardData(Guid departmentId)
        {
            DashboardData dashboardData = new()
            {
                Departments = await Task.Run(() =>
                {
                    return parkingAdministrationManager.ListDepartments().DeepCopy<Department[]>();
                }),
                AllocationResults = await Task.Run(() =>
                {
                    return parkingAllocationManager.ListAllocationResults(departmentId, dateTimeProvider.DateUtc()).DeepCopy<AllocationResult[]>();
                }),
            };

            return dashboardData;
        }

        [HttpGet("{date}/{departmentId}")]
        public async Task<AllocationResult[]> GetAllocationResult(DateTime date, Guid departmentId)
        {
            AllocationResult[] allocationResult = await Task.Run(() =>
            {
                return parkingAllocationManager.ListAllocationResults(departmentId, dateTimeProvider.DateUtc(date)).DeepCopy<AllocationResult[]>();
            });

            return allocationResult;
        }

        [HttpDelete]
        public void DeleteAvailableParkingSpot(Guid id)
        {
            parkingAdministrationManager.DeleteAvailableParkingSpot(id);
        }

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
