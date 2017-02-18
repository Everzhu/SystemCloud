using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Elective.Models.ElectiveSection
{
    public class Edit
    {
        public Entity.tbElectiveSection ElectiveSectionEdit { get; set; } = new Entity.tbElectiveSection();

        public int ElectiveId { get; set; } = System.Web.HttpContext.Current.Request["ElectiveId"].ConvertToInt();
    }
}