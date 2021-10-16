using ParkingManagement.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParkingManagement.Security
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
