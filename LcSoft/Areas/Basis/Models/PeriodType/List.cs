using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Basis.Models.PeriodType
{
    public class List
    {
        public List<Entity.tbPeriodType> PeriodTypeList { get; set; } = new List<Entity.tbPeriodType>();

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();
    }
}