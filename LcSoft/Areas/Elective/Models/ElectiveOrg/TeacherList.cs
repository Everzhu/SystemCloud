using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace XkSystem.Areas.Elective.Models.ElectiveOrg
{
    public class TeacherList
    {
        public List<Dto.ElectiveOrg.TeacherList> SysTeacherList { get; set; } = new List<Dto.ElectiveOrg.TeacherList>();

        public List<System.Web.Mvc.SelectListItem> DepartList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();

        public int DeptId { get; set; }

        public Code.PageHelper Page { get; set; } = new Code.PageHelper();
    }
}
