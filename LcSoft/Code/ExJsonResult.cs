using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Code
{
    public class ExJsonResult : JsonResult
    {
        public ExJsonResult(object obj)
        {
            base.Data = obj;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            var set = new JsonSerializerSettings();
            set.Converters.Add(new ExJsonConverter());
            set.NullValueHandling = NullValueHandling.Ignore;
            string json = JsonConvert.SerializeObject(Data, set);
            context.HttpContext.Response.ContentType = "application/json";
            context.HttpContext.Response.Write(json);
        }
    }

    public class ExJsonConverter : JsonConverter
    {

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(bool) || objectType == typeof(long) || objectType == typeof(long?) || objectType == typeof(string) || objectType.IsEnum
                || (objectType.IsGenericType && objectType.GetGenericTypeDefinition() == typeof(Nullable<>) && (objectType.GetGenericArguments()[0].IsEnum
                || objectType.GetGenericArguments()[0] == typeof(DateTime))) || objectType == typeof(DateTime) || objectType == typeof(DateTime?);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value != null)
            {
                object str = string.Empty;

                //if (value is DateTime)
                //{
                //    DateTime dt = (DateTime)value;
                //    str = DateTimeHelper.GetUNIXDateTime(dt).ToString();
                //}
                //else if (value is DateTime?)
                //{
                //    DateTime dt = (value as DateTime?).Value;
                //    str = DateTimeHelper.GetUNIXDateTime(dt).ToString();
                //}
                if (value is System.Enum)
                {
                    try
                    {
                        var t = value.GetType();
                        FieldInfo fi = t.GetField(System.Enum.GetName(t, value));
                        var attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
                        str = (attributes.Length > 0) ? attributes[0].Description : System.Enum.GetName(t, value);
                    }
                    catch { }
                }
                else if (value is long || value is long?)
                {
                    str = value.ToString();
                }
                else if (value is string)
                {
                    if (string.IsNullOrWhiteSpace(value.ToString()))
                    {
                        str = "";
                    }
                    else
                    {
                        if (value.ToString().ToLower() == "false")
                        {
                            str = "0";
                        }
                        else if (value.ToString().ToLower() == "true")
                        {
                            str = "1";
                        }
                        else
                        {
                            str = value.ToString();
                        }
                    }
                }
                else if (value is bool)
                {
                    str = ((bool)value) ? 1 : 0;
                }
                writer.WriteValue(str);// or something else
            }
            else if (value is string)
            {
                writer.WriteValue("");
            }

        }
    }
}