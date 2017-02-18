using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Basis.Models.BuildType
{
    public class List
    {
        public List<Entity.tbBuildType> DataList { get; set; } = new List<Entity.tbBuildType>();
        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();
    }
}