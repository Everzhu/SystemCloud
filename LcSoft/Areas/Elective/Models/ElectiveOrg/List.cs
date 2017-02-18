using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace XkSystem.Areas.Elective.Models.ElectiveOrg
{
    public class List
    {
        public List<Dto.ElectiveOrg.List> ElectiveOrgList { get; set; } = new List<Dto.ElectiveOrg.List>();

        public string ElectiveName { get; set; }

        public List<System.Web.Mvc.SelectListItem> ElectiveSectionList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> ElectiveGroupList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public bool IsWeekPeriod { get; set; }

        public bool IsHiddenSection { get; set; }

        public bool IsHiddenGroup { get; set; }

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();

        public int ElectiveId { get; set; } = System.Web.HttpContext.Current.Request["ElectiveId"].ConvertToInt();

        public int? ElectiveSectionId { get; set; } = System.Web.HttpContext.Current.Request["ElectiveSectionId"].ConvertToInt();

        public int? ElectiveGroupId { get; set; } = System.Web.HttpContext.Current.Request["ElectiveGroupId"].ConvertToInt();
    }
}
