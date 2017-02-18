using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace XkSystem.Areas.Elective.Models.ElectiveClass
{
    public class List
    {
        public string ElectiveName { get; set; }

        public int ElectiveId { get; set; } = System.Web.HttpContext.Current.Request["ElectiveId"].ConvertToInt();

        public string ClassIds { get; set; }
    }
}
