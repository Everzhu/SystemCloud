using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace XkSystem.Areas.Elective.Models.ElectiveSection
{
    public class List
    {
        public List<Entity.tbElectiveSection> ElectiveSectionList { get; set; } = new List<Entity.tbElectiveSection>();

        public string ElectiveName { get; set; }

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();

        public int ElectiveId { get; set; } = System.Web.HttpContext.Current.Request["ElectiveId"].ConvertToInt();
    }
}
