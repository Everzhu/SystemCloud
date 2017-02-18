using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Student.Models.StudentChange
{
    public class StudentReset
    {
        public Dto.StudentChange.StudentReset DataEdit { get; set; } = new Dto.StudentChange.StudentReset();

        public List<System.Web.Mvc.SelectListItem> ClassList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> StudentSessionList { get; set; } = new List<System.Web.Mvc.SelectListItem>();
    }
}