using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Course.Models.OrgSchedule
{
    public class Edit
    {
        public Dto.OrgSchedule.Edit OrgScheduleEdit { get; set; } = new Dto.OrgSchedule.Edit();

        public int? OrgId { get; set; } = 0;
    }
}