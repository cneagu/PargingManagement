using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ParkingManagement.Infrastructure
{
    public class MongoDataAccess : IMongoDataAccess
    {
        private readonly IMongoClient client;
        private readonly IMongoDatabase database;

        public MongoDataAccess(IMongoDBConfig config)
        {
            client = new MongoClient(config.MongoDBConnectionString);
            database = client.GetDatabase(config.DatabaseName);
        }

        public async Task InsertAsync<T>(string collectionName, T obj)
        {
            await GetCollection<T>(collectionName).InsertOneAsync(obj);
        }

        public async Task InsertManyAsync<T>(string collectionName, T[] obj)
        {
            await GetCollection<T>(collectionName).InsertManyAsync(obj);
        }

        public async Task UpsertAsync<T>(string collectionName, Expression<Func<T, bool>> filter, T obj)
        {
            await GetCollection<T>(collectionName).ReplaceOneAsync(filter, obj, new ReplaceOptions() { IsUpsert = true });
        }

        public async Task UpdateSingleItem<Model,Type>(string collectionName, Expression<Func<Model, bool>> filter, Type item)
        {
            var serializerSettings = new JsonSerializerSettings()
            {
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Ignore
            };
            var bson = new BsonDocument() { { "$set", BsonDocument.Parse(JsonConvert.SerializeObject(item, serializerSettings)) } };

            await GetCollection<Model>(collectionName).UpdateOneAsync(filter, bson);
        }

        public async Task DeleteAsync<T>(string collectionName, Expression<Func<T, bool>> filter)
        {
            await GetCollection<T>(collectionName).DeleteManyAsync(filter);
        }

        public async Task<long> CountAsync<T>(string collectionName, Expression<Func<T, bool>> filter = null)
        {
            return await BuildQueryExpression(collectionName, filter).CountDocumentsAsync();
        }

        public async Task<List<T>> SelectAsync<T>(
            string collectionName,
            Expression<Func<T, bool>> filter = null,
            Expression<Func<T, object>> sortBy = null,
            int? resultLimit = null,
            int resultsToSkip = 0)
        {
            IFindFluent<T, T> queryExpression = BuildQueryExpression(collectionName, filter, sortBy, resultLimit, resultsToSkip);

            return await queryExpression.ToListAsync();
        }

        private IFindFluent<T, T> BuildQueryExpression<T>(string collectionName, Expression<Func<T, bool>> filter = null, Expression<Func<T, object>> sortBy = null, int? resultLimit = null, int resultsToSkip = 0)
        {
            IMongoCollection<T> collection = GetCollection<T>(collectionName);
            IFindFluent<T, T> query;

            if (filter == null)
                query = collection.Find(x => true);
            else
                query = collection.Find(filter);

            if (sortBy != null)
            {
                query = query.SortBy(sortBy);
            }

            return query.Skip(resultsToSkip).Limit(resultLimit);
        }

        private IMongoCollection<T> GetCollection<T>(string collectionName)
        {
            MongoCollectionSettings collectionSettings = new()
            {
                GuidRepresentation = GuidRepresentation.Standard
            };
            return database.GetCollection<T>(collectionName, collectionSettings);
        }
    }
}
