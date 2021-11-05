using ParkingManagement.Infrastructure;

namespace ParkingManagement
{
    public class Config : IConfig
    {
        public string ConnectionString { get; set; }
        public string MongoDBConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }

    public interface IConfig : Infrastructure.IConfig, IMongoDBConfig
    {
    }
}
