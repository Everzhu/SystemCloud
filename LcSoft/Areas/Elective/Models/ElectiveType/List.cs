using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace XkSystem.Areas.Elective.Models.ElectiveType
{
    public class List
    {
        public List<Entity.tbElectiveType> ElectiveTypeList { get; set; } = new List<Entity.tbElectiveType>();

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();
    }
}
