using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Elective.Models.ElectiveApply
{
    public class Detail
    {

        public Dto.ElectiveApply.Detail ElectiveApplyDetail { get; set; } = new Dto.ElectiveApply.Detail();
        public bool IsWeekPeriod { get;  set; } = false;

        public List<SelectListItem> PeriodList { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> WeekList { get; set; } = new List<SelectListItem>();
    }
}