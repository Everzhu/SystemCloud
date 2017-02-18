using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Attendance.Models.Attendance
{
    public class StudentDetail
    {
        public Dto.Attendance.Info AttendanceInfo { get; set; } = new Dto.Attendance.Info();
        public List<Dto.Attendance.Info> AttendanceInfoList { get; set; } = new List<Dto.Attendance.Info>();
        public List<Areas.Student.Dto.Student.Info> StudentInfoList { get; set; } = new List<Areas.Student.Dto.Student.Info>();
        public List<System.Web.Mvc.SelectListItem> AttendanceTypeList { get; set; } = new List<System.Web.Mvc.SelectListItem>();
        public List<System.Web.Mvc.SelectListItem> WeekDayList { get; set; } = new List<System.Web.Mvc.SelectListItem>();
        public List<System.Web.Mvc.SelectListItem> WeekList { get; set; } = new List<System.Web.Mvc.SelectListItem>();
        public List<System.Web.Mvc.SelectListItem> PeriodList { get; set; } = new List<System.Web.Mvc.SelectListItem>();
        public List<Course.Dto.OrgSchedule.Info> OrgScheduleList { get; set; } = new List<Course.Dto.OrgSchedule.Info>();

        public List<System.Web.Mvc.SelectListItem> ClassList { get; set; } = new List<System.Web.Mvc.SelectListItem>();
        public List<System.Web.Mvc.SelectListItem> YearList { get; set; } = new List<System.Web.Mvc.SelectListItem>();
        public int YearId { get; set; } = System.Web.HttpContext.Current.Request["YearId"].ConvertToInt();
        public int ClassId { get; set; } = System.Web.HttpContext.Current.Request["ClassId"].ConvertToInt();
        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();
        public int StudentId { get; set; } = System.Web.HttpContext.Current.Request["StudentId"].ConvertToInt();
        public int DayWeekId { get; set; } = System.Web.HttpContext.Current.Request["DayWeekId"].ConvertToInt();
        public string DayNow { get; set; } = System.Web.HttpContext.Current.Request["DayNow"].ToString();
        public string DayOfWeek { get; set; }
        public string DateNow { get; set; }
    }
}