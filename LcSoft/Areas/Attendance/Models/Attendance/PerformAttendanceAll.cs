using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Attendance.Models.Attendance
{
    public class AttendanceAll
    {
        public Dto.Attendance.Info AttendanceInfo { get; set; } = new Dto.Attendance.Info();

        public List<Dto.Attendance.Info> AttendanceInfoList { get; set; } = new List<Dto.Attendance.Info>();

        public List<Dto.Attendance.Info> AttendanceInfoClassList { get; set; } = new List<Dto.Attendance.Info>();

        public List<System.Web.Mvc.SelectListItem> AttendanceTypeList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> ClassList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> YearList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public int? YearId { get; set; } = HttpContext.Current.Request["YearId"].ConvertToInt();

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();

        public string DateSearchFrom { get; set; } = Convert.ToString(HttpContext.Current.Request["DateSearchFrom"]);

        public string DateSearchTo { get; set; } = Convert.ToString(HttpContext.Current.Request["DateSearchTo"]);
    }
}