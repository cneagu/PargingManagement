using ParkingManagement.Infrastructure;
using ParkingManagement.Resource.EmployeeParkingRequest.Contract;
using System;
using System.Linq;

namespace ParkingManagement.Resource.EmployeeParkingRequest
{
    public class EmployeeParkingRequestResource : IEmployeeParkingRequestResource
    {
        private const string collectionName = "EmployeeParkingRequest";
        private readonly IMongoDataAccess mongoDataAccess;

        public EmployeeParkingRequestResource(IMongoDataAccess mongoDataAccess)
        {
            this.mongoDataAccess = mongoDataAccess;
        }

        public Contract.EmployeeParkingRequest[] SelectAll()
        {
            return mongoDataAccess.SelectAsync<Contract.EmployeeParkingRequest>(collectionName).Result.ToArray();
        }

        public Contract.EmployeeParkingRequest[] SelectAvailableByEffectiveDate(DateTime date)
        {
            return mongoDataAccess
                .SelectAsync<Contract.EmployeeParkingRequest>(
                    collectionName,
                    x => x.EffectiveDate == date && x.Accepted && !x.Allocated,
                    sortBy: x => x.Priority)
                .Result.ToArray();
        }

        public Contract.EmployeeParkingRequest[] SelectAlocatedByDepartmentAndEffectiveDate(Guid departmentId, DateTime date)
        {
            return mongoDataAccess
                .SelectAsync<Contract.EmployeeParkingRequest>(
                    collectionName,
                    x => x.EffectiveDate == date && x.DepartmentID == departmentId,
                    sortBy: x => x.Priority)
                .Result.ToArray();
        }

        public Contract.EmployeeParkingRequest[] SelectByEffectiveDate(DateTime date)
        {
            return mongoDataAccess
                .SelectAsync<Contract.EmployeeParkingRequest>(
                    collectionName,
                    x => x.EffectiveDate == date)
                .Result.ToArray();
        }

        public Contract.EmployeeParkingRequest SelectByID(Guid id)
        {
            return mongoDataAccess.SelectAsync<Contract.EmployeeParkingRequest>(collectionName, x => x.ID == id).Result.FirstOrDefault();
        }

        public void Insert(Contract.EmployeeParkingRequest employeeParkingRequest)
        {
            mongoDataAccess.InsertAsync(collectionName, employeeParkingRequest).Wait();
        }

        public void Upsert(Contract.EmployeeParkingRequest employeeParkingRequest)
        {
            mongoDataAccess.UpsertAsync(collectionName, x => x.ID == employeeParkingRequest.ID, employeeParkingRequest).Wait();
        }

        public void UpdateAllocatedFieldByID(Guid id, bool allocated)
        {
            mongoDataAccess.UpdateSingleItem<Contract.EmployeeParkingRequest, bool>(
                collectionName,
                x => x.ID == id,
                allocated);
        }
    }
}
