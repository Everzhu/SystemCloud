using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Attendance.Models.AttendanceReport
{
    public class ClassList
    {
        public List<Dto.AttendanceReport.ClassList> AttendanceClassList { get; set; } = new List<Dto.AttendanceReport.ClassList>();
        public List<System.Web.Mvc.SelectListItem> AttendanceTypeList { get; set; } = new List<System.Web.Mvc.SelectListItem>();
        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();
        public string DateSearchFrom { get; set; } = Convert.ToString(HttpContext.Current.Request["DateSearchFrom"]);
        public string DateSearchTo { get; set; } = Convert.ToString(HttpContext.Current.Request["DateSearchTo"]);
    }
}