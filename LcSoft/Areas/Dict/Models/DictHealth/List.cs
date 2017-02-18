using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Dict.Models.DictHealth
{
    public class List
    {
        public List<Entity.tbDictHealth> HealthList { get; set; } = new List<Entity.tbDictHealth>();

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();
    }
}