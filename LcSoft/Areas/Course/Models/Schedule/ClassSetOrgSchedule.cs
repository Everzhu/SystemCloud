using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Course.Models.Schedule
{
    public class ClassSetOrgSchedule
    {
        public int OriginalWeekId { get; set; }
        public int OriginalPeriodId { get; set; }
        public int OriginalOrgId { get; set; }
        public int ModifyWeekId { get; set; }
        public int ModifyPeriodId { get; set; }
        public int ModifyOrgId { get; set; }
    }
}