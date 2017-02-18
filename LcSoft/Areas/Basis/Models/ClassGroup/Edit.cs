using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Basis.Models.ClassGroup
{
    public class Edit
    {
        public Dto.ClassGroup.Edit ClassGroupEdit { get; set; } = new Dto.ClassGroup.Edit();

        public int ClassId { get; set; }

        public List<System.Web.Mvc.SelectListItem> TeacherList { get; set; } = new List<System.Web.Mvc.SelectListItem>();
    }
}