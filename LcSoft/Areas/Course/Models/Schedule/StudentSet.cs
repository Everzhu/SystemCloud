using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Course.Models.Schedule
{
    public class StudentSet
    {
        /// <summary>
        /// 学生信息
        /// </summary>
        public Areas.Student.Dto.Student.Info StudentInfo
        {
            get;
            set;
        }

        /// <summary>
        /// 学生ID
        /// </summary>
        public int StudentId
        {
            get;
            set;
        } = System.Web.HttpContext.Current.Request["StudentId"].ConvertToInt();

        /// <summary>
        /// 学年ID
        /// </summary>
        public int YearId
        {
            get;
            set;
        } = System.Web.HttpContext.Current.Request["YearId"].ConvertToInt();

        public List<System.Web.Mvc.SelectListItem> WeekList
        {
            get;
            set;
        } = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> PeriodList
        {
            get;
            set;
        } = new List<System.Web.Mvc.SelectListItem>();

        public List<Dto.OrgSchedule.Info> OrgScheduleList
        {
            get;
            set;
        } = new List<Dto.OrgSchedule.Info>();
    }
}