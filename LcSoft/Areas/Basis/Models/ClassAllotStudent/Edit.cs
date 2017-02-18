using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Basis.Models.ClassAllotStudent
{
    public class Edit
    {
        public Dto.ClassAllotStudent.Edit ClassAllotStudentEdit { get; set; } = new Dto.ClassAllotStudent.Edit();

        public List<System.Web.Mvc.SelectListItem> YearList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> GradeList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> ClassTypeList { get; set; } = new List<System.Web.Mvc.SelectListItem>();
    }
}