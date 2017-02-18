using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace XkSystem.Areas.Elective.Models.ElectiveOrgClass
{
    public class List
    {
        public List<Dto.ElectiveOrgClass.List> ElectiveOrgClassList { get; set; } = new List<Dto.ElectiveOrgClass.List>();

        public bool IsPermitClass { get; set; }

        public int ElectiveId { get; set; } = System.Web.HttpContext.Current.Request["ElectiveId"].ConvertToInt();

        public int ElectiveOrgId { get; set; } = System.Web.HttpContext.Current.Request["ElectiveOrgId"].ConvertToInt();

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();
    }
}
