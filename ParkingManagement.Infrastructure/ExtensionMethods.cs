using Newtonsoft.Json;
using System;
using System.Linq;
using System.Reflection;

namespace ParkingManagement.Infrastructure
{
    public static class ExtensionMethods
    {
        public static T Clone<T>(this T source)
        {
            if (source == null)
            {
                return default;
            }

            var deserializeSettings = new JsonSerializerSettings { ObjectCreationHandling = ObjectCreationHandling.Replace };
            var serializeSettings = new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore };
            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(source, serializeSettings), deserializeSettings);
        }

        public static T DeepCopy<T>(this object obj)
        {
            if (obj == null)
            {
                return default;
            }

            var str = JsonConvert.SerializeObject(obj);
            var ret = JsonConvert.DeserializeObject<T>(str);
            return ret;
        }
    }
}
