using ParkingManagement.Infrastructure;
using ParkingManagement.Resource.ParkingSpot.Contract;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ParkingManagement.Resource.ParkingSpot
{
    public class AvailableParkingSpotResource : IAvailableParkingSpotResource
    {
        private const string collectionName = "AvailableParkingSpot";
        private readonly IMongoDataAccess mongoDataAccess;

        public AvailableParkingSpotResource(IMongoDataAccess mongoDataAccess)
        {
            this.mongoDataAccess = mongoDataAccess;
        }

        public AvailableParkingSpot[] SelectAll()
        {
            return mongoDataAccess.SelectAsync<AvailableParkingSpot>(collectionName).Result.ToArray();
        }

        public AvailableParkingSpot[] SelectByEffectiveDate(DateTime date)
        {
            return mongoDataAccess
                .SelectAsync<AvailableParkingSpot>(
                    collectionName,
                    x => x.EffectiveDate == date && x.EmployeeParkingRequestID == null)
                .Result.ToArray();
        }

        public AvailableParkingSpot SelectByID(Guid id)
        {
            return mongoDataAccess.SelectAsync<AvailableParkingSpot>(collectionName, x => x.ID == id).Result.FirstOrDefault();
        }

        public void Upsert(AvailableParkingSpot availableParkingSpot)
        {
            mongoDataAccess.UpsertAsync(collectionName, x => x.ID == availableParkingSpot.ID, availableParkingSpot).Wait();
        }

        public List<AvailableParkingSpot> ListParkingSpotsBetweenInterval(Guid parkingSpotId, DateTime startDate, DateTime endDate)
        {
            return mongoDataAccess.SelectAsync<AvailableParkingSpot>(collectionName, (x => x.ParkingSpotID == parkingSpotId && x.EffectiveDate >= startDate && x.EffectiveDate <= endDate)).Result.ToList();
        }

        public AvailableParkingSpot[] ListAvailableParkingSpot(Guid parkingSpotId, DateTime date)
        {
            return mongoDataAccess.SelectAsync<AvailableParkingSpot>(collectionName, (x => x.ParkingSpotID == parkingSpotId && x.EffectiveDate >= date && x.EmployeeParkingRequestID == null)).Result.ToArray();
        }

        public AvailableParkingSpot[] ListAvailableParkingSpotsByDepartment(Guid departmentID, DateTime date)
        {
            return mongoDataAccess.SelectAsync<AvailableParkingSpot>(collectionName, (x => x.DepartmentID == departmentID && x.EffectiveDate == date)).Result.ToArray();
        }

        public void DeleteById(Guid id)
        {
            mongoDataAccess.DeleteAsync<AvailableParkingSpot>(collectionName, (x => x.ID == id));
        }

        public void UpdateEmployeeParkingRequestFieldByID(Guid id, Guid? employeeParkingRequestId)
        {
            mongoDataAccess.UpdateSingleItem<AvailableParkingSpot, Guid?>(
                collectionName,
                x => x.ID == id,
                employeeParkingRequestId);
        }
    }
}
