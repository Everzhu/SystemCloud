using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Elective.Models.ElectiveOrg
{
    public class Select
    {
        public List<Dto.ElectiveOrg.Select> ElectiveOrgList { get; set; } = new List<Dto.ElectiveOrg.Select>();

        public bool IsOpen { get; set; }

        public bool IsEnd { get; set; }

        public int ElectiveId { get; set; } = System.Web.HttpContext.Current.Request["ElectiveId"].ConvertToInt();

        public int WeekId { get; set; } = System.Web.HttpContext.Current.Request["WeekId"].ConvertToInt();

        public int PeriodId { get; set; } = System.Web.HttpContext.Current.Request["PeriodId"].ConvertToInt();

        public int GroupId { get; set; } = System.Web.HttpContext.Current.Request["GroupId"].ConvertToInt();

        public int SectionId { get; set; } = System.Web.HttpContext.Current.Request["SectionId"].ConvertToInt();

        public int ElectiveOrgId { get; set; } = System.Web.HttpContext.Current.Request["ElectiveOrgId"].ConvertToInt();
    }
}