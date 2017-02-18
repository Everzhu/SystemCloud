using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Basis.Models.Period
{
    public class List
    {
        public List<Dto.Period.List> PeriodList { get; set; } = new List<Dto.Period.List>();

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();
    }
}