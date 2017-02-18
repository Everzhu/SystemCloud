using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace XkSystem.Areas.Elective.Models.ElectiveGroup
{
    public class List
    {
        public List<Entity.tbElectiveGroup> ElectiveGroupList { get; set; } = new List<Entity.tbElectiveGroup>();

        public string ElectiveName { get; set; }

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();

        public int ElectiveId { get; set; } = System.Web.HttpContext.Current.Request["ElectiveId"].ConvertToInt();
    }
}
