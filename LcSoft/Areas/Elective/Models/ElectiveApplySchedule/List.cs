using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Elective.Models.ElectiveApplySchedule
{
    public class List
    {
        public List<Dto.ElectiveApplySchedule.List> ScheduleList { get; set; } = new List<Dto.ElectiveApplySchedule.List>();

        public List<SelectListItem> WeekList { get; set; } = new List<SelectListItem>();

        public List<SelectListItem> PeriodList { get; set; } = new List<SelectListItem>();

    }
}