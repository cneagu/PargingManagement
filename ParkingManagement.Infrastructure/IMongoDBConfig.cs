namespace ParkingManagement.Infrastructure
{
    public interface IMongoDBConfig
    {
        string MongoDBConnectionString { get; }
        string DatabaseName { get; }
    }
}
