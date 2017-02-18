using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Course.Models.Org
{
    public class WeekPeriodList
    {
        public List<Entity.tbOrgSchedule> OrgScheduleList { get; set; } = new List<Entity.tbOrgSchedule>();

        public List<System.Web.Mvc.SelectListItem> WeekList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> PeriodList { get; set; }

        public List<System.Web.Mvc.SelectListItem> PeriodTypeList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public string WeekPeriodId { get; set; }

        public int? OrgId { get; set; }
    }
}