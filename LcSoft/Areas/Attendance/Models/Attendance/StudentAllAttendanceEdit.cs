using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Attendance.Models.Attendance
{
    public class StudentAllAttendanceEdit
    {
        public Dto.Attendance.StudentAllAttendanceEdit StudentAttendanceEdit { get; set; } = new Dto.Attendance.StudentAllAttendanceEdit();

        public List<System.Web.Mvc.SelectListItem> WeekList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> PeriodList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> AttendanceTypeList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public int StudentId { get; set; } = System.Web.HttpContext.Current.Request["StudentId"].ConvertToInt();

        public int YearId { get; set; } = System.Web.HttpContext.Current.Request["YearId"].ConvertToInt();

        public int fromDate { get; set; } = System.Web.HttpContext.Current.Request["fromDate"].ConvertToInt();

        public int toDate { get; set; } = System.Web.HttpContext.Current.Request["toDate"].ConvertToInt();

        public int typeId { get; set; } = System.Web.HttpContext.Current.Request["typeId"].ConvertToInt();
    }
}