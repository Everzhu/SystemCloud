using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Basis.Models.ClassType
{
    public class List
    {
        public List<Entity.tbClassType> ClassTypeList { get; set; } = new List<Entity.tbClassType>();

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();
    }
}