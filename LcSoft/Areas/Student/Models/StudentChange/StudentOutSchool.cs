using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Student.Models.StudentChange
{
    public class StudentOutSchool
    {
        public Dto.StudentChange.StudentOutSchool DataEdit { get; set; } = new Dto.StudentChange.StudentOutSchool();

        public List<System.Web.Mvc.SelectListItem> StudentChangeTypeList { get; set; } = new List<System.Web.Mvc.SelectListItem>();
    }
}