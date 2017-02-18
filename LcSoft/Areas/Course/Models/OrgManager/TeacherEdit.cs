using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Course.Models.OrgManager
{
    public class TeacherEdit
    {
        public Dto.OrgManager.TeacherEdit DataEdit { get; set; } = new Dto.OrgManager.TeacherEdit();

        public List<System.Web.Mvc.SelectListItem> TeacherList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public Code.PageHelper Page { get; set; } = new Code.PageHelper();

        public List<Dto.OrgManager.EditOrgList> OrgList { get; set; } = new List<Dto.OrgManager.EditOrgList>();
    }
}