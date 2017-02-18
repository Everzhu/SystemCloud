using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Course.Models.Schedule
{
    public class ClassAll
    {
        public List<Dto.OrgSchedule.Info> OrgScheduleList { get; set; } = new List<Dto.OrgSchedule.Info>();

        public List<System.Web.Mvc.SelectListItem> WeekList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> PeriodList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<XkSystem.Areas.Basis.Dto.Period.List> PeriodList2 { get; set; } = new List<Basis.Dto.Period.List>();

        public List<System.Web.Mvc.SelectListItem> ClassList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> TeacherList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<Areas.Teacher.Dto.Teacher.Info> TeacherInfoList { get; set; } = new List<Teacher.Dto.Teacher.Info>();

        public List<System.Web.Mvc.SelectListItem> StudentList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<Areas.Student.Dto.Student.Info> StudentInfoList { get; set; } = new List<Student.Dto.Student.Info>();

        public List<System.Web.Mvc.SelectListItem> RoomList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> YearList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();

        public int? YearId { get; set; } = System.Web.HttpContext.Current.Request["YearId"].ConvertToInt();

        /// <summary>
        /// 行政班级
        /// </summary>
        public List<Areas.Basis.Dto.Class.Info> ClassInfoList { get; set; } = new List<Basis.Dto.Class.Info>();

        /// <summary>
        /// 行政班级Id
        /// </summary>
        public int? ClassId { get; set; } = System.Web.HttpContext.Current.Request["ClassId"].ConvertToInt();

        public int? Id { get; set; } = System.Web.HttpContext.Current.Request["Id"].ConvertToInt();
    }
}