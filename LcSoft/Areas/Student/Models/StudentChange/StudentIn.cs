using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Student.Models.StudentChange
{
    public class StudentIn
    {
        public Dto.StudentChange.StudentIn DataEdit { get; set; } = new Dto.StudentChange.StudentIn();

        public List<System.Web.Mvc.SelectListItem> ClassList { get; set; } = new List<System.Web.Mvc.SelectListItem>();
    }
}