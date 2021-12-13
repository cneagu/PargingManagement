using ParkingManagement.Contract.ParkingAdministration;
using ParkingManagement.Infrastructure;
using ParkingManagement.Resource.Department.Contract;
using ParkingManagement.Resource.ParkingSpot.Contract;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ParkingManagement.Manager
{
    public class ParkingAdministrationManager : IParkingAdministrationManager
    {
        private readonly IAvailableParkingSpotResource availableParkingSpotResource;
        private readonly IParkingSpotResource parkingSpotResource;
        private readonly IDateTimeProvider dateTimeProvider;
        private readonly IDepartmentResource departmentResource;

        public ParkingAdministrationManager(
            IAvailableParkingSpotResource availableParkingSpotResource,
            IParkingSpotResource parkingSpotResource,
            IDateTimeProvider dateTimeProvider,
            IDepartmentResource departmentResource)
        {
            this.availableParkingSpotResource = availableParkingSpotResource;
            this.parkingSpotResource = parkingSpotResource;
            this.dateTimeProvider = dateTimeProvider;
            this.departmentResource = departmentResource;
        }
        public DateInputStatus AddAvailableParkingSpot(Guid employeeId, DateInput dateInput)
        {
            ParkingSpot spot = parkingSpotResource.SelectByEmployeeID(employeeId);
            Guid parkingSpotID = spot.ID;

            List<DateTime> dateRange = GetDateRange(dateTimeProvider.DateTimeUtc(dateInput.StartDate).Date, dateTimeProvider.DateTimeUtc(dateInput.EndDate).Date, parkingSpotID);

            foreach (DateTime date in dateRange.Reverse<DateTime>())
            {
                availableParkingSpotResource.Upsert(new Resource.ParkingSpot.Contract.AvailableParkingSpot
                {
                    ID = Guid.NewGuid(),
                    ParkingSpotID = parkingSpotID,
                    EffectiveDate = dateTimeProvider.DateTimeUtc(date),
                    DepartmentID = spot.DepartmentID
                });
            }

            if (dateRange.Count == 0)
                return DateInputStatus.NoDate;

            return DateInputStatus.Success;
        }

        public Contract.ParkingAdministration.AvailableParkingSpot[] ListAvailableParkingSpot(Guid employeeId, DateTime effectiveDate)
        {
            ParkingSpot spot = parkingSpotResource.SelectByEmployeeID(employeeId);
            Resource.ParkingSpot.Contract.AvailableParkingSpot[] availableParkingSpotList = availableParkingSpotResource.ListAvailableParkingSpot(spot.ID, effectiveDate);
            return availableParkingSpotList.DeepCopy<Contract.ParkingAdministration.AvailableParkingSpot[]>();
        }

        public void DeleteAvailableParkingSpot(Guid parkingSpotId)
        {
            availableParkingSpotResource.DeleteById(parkingSpotId);
        }

        public Contract.ParkingAdministration.Department[] ListDepartments()
        {
            return departmentResource.SelectAll().DeepCopy<Contract.ParkingAdministration.Department[]>();
        }

        private List<DateTime> GetDateRange(DateTime startDate, DateTime endDate, Guid parkingSpotID)
        {
            List<DateTime> dateRange = new List<DateTime>();
            List<Resource.ParkingSpot.Contract.AvailableParkingSpot> parkingSpots = availableParkingSpotResource.ListParkingSpotsBetweenInterval(parkingSpotID, startDate, endDate).OrderBy(x => x.EffectiveDate).ToList();
            int availableParkingSpotIndex = 0;

            while (startDate <= endDate)
            {
                if (startDate.DayOfWeek == DayOfWeek.Saturday)
                {
                    startDate = startDate.AddDays(2);
                    continue;
                }

                if (startDate.DayOfWeek == DayOfWeek.Sunday)
                {
                    startDate = startDate.AddDays(1);
                    continue;
                }

                bool existsRecordForParkingSpot = availableParkingSpotIndex < parkingSpots.Count && startDate == parkingSpots[availableParkingSpotIndex].EffectiveDate;

                if (existsRecordForParkingSpot)
                {
                    availableParkingSpotIndex++;
                    startDate = startDate.AddDays(1);
                    continue;
                }

                dateRange.Add(startDate);
                startDate = startDate.AddDays(1);
            }

            return dateRange;
        }
    }
}
