using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Basis.Models.ClassAllotClass
{
    public class List
    {
        public List<Dto.ClassAllotClass.List> ClassAllotClassList { get; set; } = new List<Dto.ClassAllotClass.List>();

        public List<System.Web.Mvc.SelectListItem> YearList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();

        public int? YearId { get; set; } = System.Web.HttpContext.Current.Request["YearId"].ConvertToInt();
    }
}