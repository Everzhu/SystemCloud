using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Attendance.Models.Attendance
{
    public class StudentAllAttendance
    {
        public Dto.Attendance.Info AttendanceInfo { get; set; } = new Dto.Attendance.Info();

        public List<Dto.Attendance.Info> AttendanceInfoList { get; set; } = new List<Dto.Attendance.Info>();

        public List<Areas.Student.Dto.Student.List> StudentInfoList { get; set; } = new List<Areas.Student.Dto.Student.List>();

        public List<System.Web.Mvc.SelectListItem> AttendanceTypeList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> ClassList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> YearList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public int YearId { get; set; } = System.Web.HttpContext.Current.Request["YearId"].ConvertToInt();

        public int ClassId { get; set; } = System.Web.HttpContext.Current.Request["ClassId"].ConvertToInt();

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();
    }
}