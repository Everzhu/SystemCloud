using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Code
{
    public class JsonHelper
    {
        public static string ToJsonString<T>(T value)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(value);
        }

        public static T FromJsonString<T>(string json) where T:new()
        {
            try
            {
                return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json);
            }
            catch
            {
                return new T();
            }
        }
    }
}