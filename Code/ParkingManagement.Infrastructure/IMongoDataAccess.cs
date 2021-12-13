using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ParkingManagement.Infrastructure
{
    public interface IMongoDataAccess
    {
        Task<long> CountAsync<T>(string collectionName, Expression<Func<T, bool>> filter = null);
        Task UpdateSingleItem<Model, Type>(string collectionName, Expression<Func<Model, bool>> filter, Type item);
        Task DeleteAsync<T>(string collectionName, Expression<Func<T, bool>> filter);
        Task InsertAsync<T>(string collectionName, T obj);
        Task InsertManyAsync<T>(string collectionName, T[] obj);
        Task<List<T>> SelectAsync<T>(string collectionName, Expression<Func<T, bool>> filter = null, Expression<Func<T, object>> sortBy = null, int? resultLimit = null, int resultsToSkip = 0);
        Task UpsertAsync<T>(string collectionName, Expression<Func<T, bool>> filter, T obj);
    }
}