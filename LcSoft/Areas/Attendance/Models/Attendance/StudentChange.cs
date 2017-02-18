using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Attendance.Models.Attendance
{
    public class StudentChange
    {
        public Dto.Attendance.StudentChange StudentChangeInfo { get; set; } = new Dto.Attendance.StudentChange();

        public List<System.Web.Mvc.SelectListItem> YearList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<Dto.Attendance.StudentChange> StudentChangeList { get; set; } = new List<Dto.Attendance.StudentChange>();

        public List<Areas.Student.Dto.Student.List> StudentInfoList { get; set; } = new List<Areas.Student.Dto.Student.List>();

        public List<System.Web.Mvc.SelectListItem> AttendanceTypeList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> ClassList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public int ClassId { get; set; } = System.Web.HttpContext.Current.Request["ClassId"].ConvertToInt();

        public int YearId { get; set; } = System.Web.HttpContext.Current.Request["YearId"].ConvertToInt();

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();

        public string DateSearchFrom { get; set; } = Convert.ToString(HttpContext.Current.Request["DateSearchFrom"]);

        public string DateSearchTo { get; set; } = Convert.ToString(HttpContext.Current.Request["DateSearchTo"]);
    }
}