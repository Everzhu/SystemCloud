using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Perform.Models.PerformComment
{
    public class ReportAll
    {
        public List<Dto.PerformComment.ReportAll> ReportAllList { get; set; } = new List<Dto.PerformComment.ReportAll>();

        public List<System.Web.Mvc.SelectListItem> YearList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> ClassList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public int YearId { get; set; } = HttpContext.Current.Request["YearId"].ConvertToInt();

        public int? ClassId { get; set; } = HttpContext.Current.Request["ClassId"].ConvertToInt();
    }
}