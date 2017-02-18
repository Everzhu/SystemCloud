using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Perform.Models.PerformGroup
{
    public class Edit
    {
        public Entity.tbPerformGroup PerformGroupEdit { get; set; } = new Entity.tbPerformGroup();

        public int PerformId { get; set; } = System.Web.HttpContext.Current.Request["PerformId"].ConvertToInt();
    }
}