using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Attendance.Models.Attendance
{
    public class ReportClass
    {
        public List<Dto.Attendance.My> MyAttendanceList { get; set; } = new List<Dto.Attendance.My>();

        public List<System.Web.Mvc.SelectListItem> ClassList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> CourseList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public DateTime DateSearchFrom { get; set; } = HttpContext.Current.Request["DateSearchFrom"].ConvertToDateTime();

        public DateTime DateSearchTo { get; set; } = HttpContext.Current.Request["DateSearchTo"].ConvertToDateTime();

        public int ClassId { get; set; } = HttpContext.Current.Request["ClassId"].ConvertToInt();

        public int? CourseId { get; set; } = HttpContext.Current.Request["CourseId"].ConvertToInt();

        public string SearchText { get; set; } = HttpContext.Current.Request["SearchText"].ConvertToString();
    }
}