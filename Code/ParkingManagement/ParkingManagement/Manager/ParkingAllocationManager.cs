using Microsoft.Extensions.Logging;
using ParkingManagement.Contract.ParkingAllocation;
using ParkingManagement.Infrastructure;
using ParkingManagement.Resource.Employee.Contract;
using ParkingManagement.Resource.EmployeeParkingRequest.Contract;
using ParkingManagement.Resource.ParkingSpot.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

namespace ParkingManagement.Manager
{
    public class ParkingAllocationManager : IParkingAllocationManager
    {
        private readonly ILogger logger;
        private readonly IDateTimeProvider dateTimeProvider;
        private readonly IEmployeeParkingRequestResource employeeParkingRequestResource;
        private readonly IAvailableParkingSpotResource availableParkingSpotResource;
        private readonly IParkingSpotResource parkingSpotResource;
        private readonly IEmployeeResource employeeResource;

        public ParkingAllocationManager(
            ILogger<ParkingAllocationManager> logger,
            IDateTimeProvider dateTimeProvider,
            IEmployeeParkingRequestResource employeeParkingRequestResource,
            IAvailableParkingSpotResource availableParkingSpotResource,
            IParkingSpotResource parkingSpotResource,
            IEmployeeResource employeeResource
            )
        {
            this.logger = logger;
            this.dateTimeProvider = dateTimeProvider;
            this.employeeParkingRequestResource = employeeParkingRequestResource;
            this.availableParkingSpotResource = availableParkingSpotResource;
            this.parkingSpotResource = parkingSpotResource;
            this.employeeResource = employeeResource;
        }

        public ParkingRequestStatus ResolveParkingRequest(Guid requestID, Contract.ParkingAllocation.PreferredType type)
        {
            EmployeeParkingRequest request = employeeParkingRequestResource.SelectByID(requestID);

            if (request == null)
                return ParkingRequestStatus.Failed;

            if (dateTimeProvider.DateUtc() > request.EffectiveDate || request.Allocated)
                return ParkingRequestStatus.Failed;

            request.PreferredType = (Resource.EmployeeParkingRequest.Contract.PreferredType)type;
            request.Accepted = true;

            employeeParkingRequestResource.Upsert(request);

            return ParkingRequestStatus.Success;
        }

        public AllocationResult[] AllocateParkingSpots()
        {
            DateTime effectiveDate = dateTimeProvider.DateUtc();

            EmployeeParkingRequest[] employeeParkingRequests = employeeParkingRequestResource
                .SelectAvailableByEffectiveDate(effectiveDate);

            if (!employeeParkingRequests.Any())
                return Array.Empty<AllocationResult>();

            AvailableParkingSpot[] availableParkingSpots = availableParkingSpotResource
               .SelectByEffectiveDate(effectiveDate);

            if (!availableParkingSpots.Any())
                return Array.Empty<AllocationResult>();

            ParkingSpot[] parkingSpots = parkingSpotResource
                .SelectAllByIDs(availableParkingSpots.Select(x => x.ParkingSpotID));

            List<AllocationResult> allocationResults = new();
            foreach (AvailableParkingSpot availableSpot in availableParkingSpots)
            {
                AllocateParkingSpot(allocationResults, availableSpot, effectiveDate, parkingSpots, employeeParkingRequests);
            }

            return allocationResults.ToArray();
        }

        private void AllocateParkingSpot(
            List<AllocationResult> allocationResults,
            AvailableParkingSpot availableParkingSpot,
            DateTime effectiveDate,
            ParkingSpot[] parkingSpots,
            EmployeeParkingRequest[] employeeParkingRequests)
        {
            try
            {
                ParkingSpot spot = parkingSpots.First(x => x.ID == availableParkingSpot.ParkingSpotID);

                EmployeeParkingRequest request = employeeParkingRequests
                    .FirstOrDefault(x => !x.Allocated
                        && DoesTypeMatch(x.PreferredType, spot.Type)
                        && x.DepartmentID == spot.DepartmentID);

                if (request == null)
                    return;

                using (TransactionScope scope = new(TransactionScopeOption.RequiresNew,
                        new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
                {
                    employeeParkingRequestResource.UpdateAllocatedFieldByID(request.ID, true);
                    availableParkingSpotResource.UpdateEmployeeParkingRequestFieldByID(availableParkingSpot.ID, request.ID);

                    scope.Complete();
                }

                Employee employee = employeeResource.SelectByID(request.EmployeeID);
                allocationResults.Add(new AllocationResult
                {
                    EmployeeName = string.Format("{0} {1}", employee.FirstName, employee.LastName),
                    Email = employee.Email,
                    ParkingSpotNumber = spot.Number,
                    Date = effectiveDate
                });

                request.Allocated = true;
            }
            catch (Exception ex)
            {
                logger.LogInformation(ex.Message);
            }
        }

        public Notification[] NotifyAvailableParkingSpots()
        {
            DateTime effectiveDate = dateTimeProvider.DateUtc().AddDays(1);

            Employee[] employees = GetEmployees(effectiveDate);

            List<Notification> notifications = new();
            foreach (Employee employee in employees)
            {
                AddNotification(notifications, effectiveDate, employee);
            }

            return notifications.ToArray();
        }

        public Notification[] NotifyAvailableParkingSpotsDynamic()
        {
            DateTime effectiveDate = dateTimeProvider.DateUtc();

            Employee[] employees = GetEmployees(effectiveDate);

            EmployeeParkingRequest[] requests = employeeParkingRequestResource.SelectByEffectiveDate(effectiveDate);

            List<Notification> notifications = new();
            foreach (Employee employee in employees)
            {
                if (requests.Any(request => request.EmployeeID == employee.ID))
                    continue;

                AddNotification(notifications, effectiveDate, employee);
            }

            return notifications.ToArray();
        }

        public AllocationResult[] ListAllocationResults(Guid departmentId, DateTime effectiveDate)
        {
            ParkingSpot[] parkingSpots = parkingSpotResource.SelectByDepartmentID(departmentId);
            Employee[] employees = employeeResource.SelectByDepartmentID(departmentId);
            List<AllocationResult> allocationResults = GetExistingParkingSpots(parkingSpots, employees);

            EmployeeParkingRequest[] employeeParkingRequests = employeeParkingRequestResource.SelectAlocatedByDepartmentAndEffectiveDate(departmentId, effectiveDate);
            AvailableParkingSpot[] availableParkingSpots = availableParkingSpotResource.ListAvailableParkingSpotsByDepartment(departmentId, effectiveDate);

            foreach (AvailableParkingSpot availableParkingSpot in availableParkingSpots)
            {
                if (availableParkingSpot.EmployeeParkingRequestID == null)
                {
                    allocationResults.First(x => x.ID == availableParkingSpot.ParkingSpotID).EmployeeName = string.Empty;
                    continue;
                }

                EmployeeParkingRequest employeeParkingRequest = employeeParkingRequests.First(request => request.ID == availableParkingSpot.EmployeeParkingRequestID);
                Employee allocatedEmployee = employees.First(employee => employee.ID == employeeParkingRequest.EmployeeID);

                allocationResults.First(x => x.ID == availableParkingSpot.ParkingSpotID).EmployeeName = string.Format("{0} {1}", allocatedEmployee.FirstName, allocatedEmployee.LastName);
            }

            return allocationResults.ToArray();
        }

        private Employee[] GetEmployees(DateTime effectiveDate)
        {
            AvailableParkingSpot[] availableParkingSpots = availableParkingSpotResource.SelectByEffectiveDate(effectiveDate);

            IEnumerable<Guid> departmentIDs = availableParkingSpots.Select(x => x.DepartmentID);
            if (!departmentIDs.Any())
                return Array.Empty<Employee>();

            return employeeResource.SelectEligibleByDepartments(departmentIDs.Distinct());
        }

        private void AddNotification(List<Notification> notifications, DateTime effectiveDate, Employee employee)
        {
            Guid employeeParkingRequestID = Guid.NewGuid();

            AddEmployeeParkingRequest(employeeParkingRequestID, effectiveDate, employee);

            notifications.Add(new Notification
            {
                EmployeeName = string.Format("{0} {1}", employee.FirstName, employee.LastName),
                Email = employee.Email,
                Date = effectiveDate,
                LinkId = employeeParkingRequestID
            });
        }

        private void AddEmployeeParkingRequest(Guid id, DateTime effectiveDate, Employee employee)
        {
            EmployeeParkingRequest employeeParkingRequest = new()
            {
                ID = id,
                EmployeeID = employee.ID,
                PreferredType = Resource.EmployeeParkingRequest.Contract.PreferredType.Any,
                EffectiveDate = effectiveDate,
                Accepted = false,
                Allocated = false,
                Priority = (double)employee.Priority,
                DepartmentID = employee.DepartmentID
            };

            employeeParkingRequestResource.Insert(employeeParkingRequest);
        }

        private bool DoesTypeMatch(Resource.EmployeeParkingRequest.Contract.PreferredType preferredType, Resource.ParkingSpot.Contract.Type spotType)
        {
            if (preferredType == Resource.EmployeeParkingRequest.Contract.PreferredType.Single)
                return spotType == Resource.ParkingSpot.Contract.Type.Single;

            return true;
        }

        private List<AllocationResult> GetExistingParkingSpots(ParkingSpot[] parkingSpots, Employee[] employees)
        {
            List<AllocationResult> existingParkingSpots = new();

            foreach (ParkingSpot parkingSpot in parkingSpots)
            {
                Employee employee = employees.FirstOrDefault(x => x.ID == parkingSpot.EmployeeID);

                AllocationResult existingParkingSpot = new()
                {
                    ID = parkingSpot.ID,
                    ParkingSpotNumber = parkingSpot.Number,
                    EmployeeName = employee != null ? string.Format("{0} {1}", employee.FirstName, employee.LastName) : string.Empty
                };

                existingParkingSpots.Add(existingParkingSpot);
            }

            return existingParkingSpots;
        }
    }
}
