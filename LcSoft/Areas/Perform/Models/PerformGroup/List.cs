using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Perform.Models.PerformGroup
{
    public class List
    {
        public List<Entity.tbPerformGroup> PerformGroupList { get; set; } = new List<Entity.tbPerformGroup>();

        public string PerformName { get; set; }

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();

        public int PerformId { get; set; } = System.Web.HttpContext.Current.Request["PerformId"].ConvertToInt();
    }
}