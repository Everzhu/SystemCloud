using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Basis.Models.ClassManager
{
    public class Edit
    {
        public Dto.ClassManager.Edit DataEdit { get; set; } = new Dto.ClassManager.Edit();

        public List<System.Web.Mvc.SelectListItem> TeacherList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public Code.PageHelper Page { get; set; } = new Code.PageHelper();

        public List<Dto.ClassManager.EditClassList> ClassList { get; set; } = new List<Dto.ClassManager.EditClassList>();
    }
}