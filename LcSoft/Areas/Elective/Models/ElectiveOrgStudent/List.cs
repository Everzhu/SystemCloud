using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace XkSystem.Areas.Elective.Models.ElectiveOrgStudent
{
    public class List
    {
        public List<Dto.ElectiveOrgStudent.List> ElectiveOrgStudentList { get; set; } = new List<Dto.ElectiveOrgStudent.List>();

        public string ElectiveOrgName { get; set; }

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();

        public int ElectiveId { get; set; } = System.Web.HttpContext.Current.Request["ElectiveId"].ConvertToInt();

        public int ElectiveOrgId { get; set; } = System.Web.HttpContext.Current.Request["ElectiveOrgId"].ConvertToInt();
    }
}
