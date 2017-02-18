using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Perform.Models.PerformComment
{
    public class ReportClass
    {
        public List<Dto.PerformComment.ReportClass> ReportClassList { get; set; } = new List<Dto.PerformComment.ReportClass>();

        public List<System.Web.Mvc.SelectListItem> YearList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> ClassList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public int YearId { get; set; } = HttpContext.Current.Request["YearId"].ConvertToInt();

        public int? ClassId { get; set; } = HttpContext.Current.Request["ClassId"].ConvertToInt();

        public string SearchText { get; set; } = HttpContext.Current.Request["SearchText"].ConvertToString();
    }
}