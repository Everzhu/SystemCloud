using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Study.Models.StudyReport
{
    public class FullList
    {
        public List<Dto.StudyReport.FullList> StudyReportFullList { get; set; } = new List<Dto.StudyReport.FullList>();
        public string DateSearchFrom { get; set; } = Convert.ToString(HttpContext.Current.Request["DateSearchFrom"]);
        public string DateSearchTo { get; set; } = Convert.ToString(HttpContext.Current.Request["DateSearchTo"]);
        public int StudyId { get; set; } = System.Web.HttpContext.Current.Request["StudyId"].ConvertToInt();
        public int ClassId { get; set; } = System.Web.HttpContext.Current.Request["ClassId"].ConvertToInt();
    }
}