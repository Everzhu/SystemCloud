using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace XkSystem.Areas.Elective.Models.ElectiveOrgSchedule
{
    public class List
    {
        public List<Dto.ElectiveOrgSchedule.List> ElectiveOrgScheduleList { get; set; } = new List<Dto.ElectiveOrgSchedule.List>();

        public List<System.Web.Mvc.SelectListItem> WeekList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> PeriodList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();

        public int ElectiveOrgId { get; set; } = System.Web.HttpContext.Current.Request["ElectiveOrgId"].ConvertToInt();
    }
}
