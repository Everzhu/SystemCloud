using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Attendance.Models.Attendance
{
    public class List
    {
        public List<Dto.Attendance.List> AttendanceList { get; set; } = new List<Dto.Attendance.List>();

        public List<Dto.Attendance.List> AttendanceAllList { get; set; } = new List<Dto.Attendance.List>();

        public List<System.Web.Mvc.SelectListItem> AttendanceTypeList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<Areas.Course.Dto.OrgSchedule.List> OrgScheduleList { get; set; } = new List<Areas.Course.Dto.OrgSchedule.List>();

        public List<Perform.Dto.PerformData.OrgSelectInfo> OrgSelectInfo { get; set; } = new List<Perform.Dto.PerformData.OrgSelectInfo>();

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();

        public int DayWeekId { get; set; } = System.Web.HttpContext.Current.Request["DayWeekId"].ConvertToInt();

        public int PeriodId { get; set; } = System.Web.HttpContext.Current.Request["PeriodId"].ConvertToInt();

        public int OrgId { get; set; } = System.Web.HttpContext.Current.Request["OrgId"].ConvertToInt();

        /// <summary>
        /// 是否班主任
        /// </summary>
        public bool IsClassTeacher { get; set; }
}
}