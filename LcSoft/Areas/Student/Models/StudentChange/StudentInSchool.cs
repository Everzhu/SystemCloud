using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Student.Models.StudentChange
{
    public class StudentInSchool
    {
        public Dto.StudentChange.StudentInSchool DataEdit { get; set; } = new Dto.StudentChange.StudentInSchool();

        public List<System.Web.Mvc.SelectListItem> ClassList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> StudentChangeTypeList { get; set; } = new List<System.Web.Mvc.SelectListItem>();
    }
}