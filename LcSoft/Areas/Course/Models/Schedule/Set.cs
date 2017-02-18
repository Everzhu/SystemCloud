using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Course.Models.Schedule
{
    public class Set
    {
        /// <summary>
        /// 学年
        /// </summary>
        public List<System.Web.Mvc.SelectListItem> YearList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> WeekList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> PeriodList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> ClassList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<Dto.OrgSchedule.Info> OrgScheduleList { get; set; } = new List<Dto.OrgSchedule.Info>();

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();

        /// <summary>
        /// 学年ID
        /// </summary>
        public int YearId { get; set; } = System.Web.HttpContext.Current.Request["YearId"].ConvertToInt();
    }
}