using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XkSystem.Areas.Teacher.Models.TeacherDept
{
    public class Edit
    {
        public Dto.TeacherDept.Edit TeacherDeptEdit { get; set; } = new Dto.TeacherDept.Edit();

        public List<System.Web.Mvc.SelectListItem> TeacherDeptParentList { get; set; } = new List<System.Web.Mvc.SelectListItem>();
    }
}
