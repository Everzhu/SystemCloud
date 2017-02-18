using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Basis.Models.Week
{
    public class List
    {
        public List<Entity.tbWeek> WeekList { get; set; } = new List<Entity.tbWeek>();

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();
    }
}