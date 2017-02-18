using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Elective.Models.ElectiveGroup
{
    public class Edit
    {
        public Entity.tbElectiveGroup ElectiveGroupEdit { get; set; } = new Entity.tbElectiveGroup();

        public int ElectiveId { get; set; } = System.Web.HttpContext.Current.Request["ElectiveId"].ConvertToInt();
    }
}