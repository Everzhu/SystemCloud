using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Course.Models.OrgStudent
{
    public class Edit
    {
        public Dto.OrgStudent.Edit OrgStudentEdit { get; set; } = new Dto.OrgStudent.Edit();

        public List<System.Web.Mvc.SelectListItem> OrgList { get; set; } = new List<System.Web.Mvc.SelectListItem>();
    }
}