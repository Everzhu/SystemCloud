using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Elective.Dto.ElectiveApplySchedule
{
    public class List
    {
        public int WeekId { get; set; }

        public string WeekName { get; set; }

        public int PeriodId { get; set; }

        public string PeriodName { get; set; }
    }

}