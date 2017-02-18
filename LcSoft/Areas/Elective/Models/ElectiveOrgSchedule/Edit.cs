using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace XkSystem.Areas.Elective.Models.ElectiveOrgSchedule
{
    public class Edit
    {
        //public Dto.ElectiveOrgSchedule.Edit ElectiveOrgScheduleEdit { get; set; } = new Dto.ElectiveOrgSchedule.Edit();

        public int ElectiveOrgId { get; set; } = System.Web.HttpContext.Current.Request["ElectiveOrgId"].ConvertToInt();
    }
}
