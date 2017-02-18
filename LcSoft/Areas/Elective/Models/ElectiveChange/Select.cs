using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Elective.Models.ElectiveChange
{
    public class Select
    {
        public List<Dto.ElectiveChange.Select> ElectiveOrgList { get; set; } = new List<Dto.ElectiveChange.Select>();

        public bool IsWeekPeriod { get; set; }

        public int ElectiveId { get; set; } = System.Web.HttpContext.Current.Request["ElectiveId"].ConvertToInt();

        public int ElectiveOrgId { get; set; } = System.Web.HttpContext.Current.Request["ElectiveOrgId"].ConvertToInt();

        public int UserId { get; set; } = System.Web.HttpContext.Current.Request["UserId"].ConvertToInt();
    }
}