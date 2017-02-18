using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Student.Models.StudentChange
{
    public class StudentOut
    {
        public Dto.StudentChange.StudentOut DataEdit { get; set; } = new Dto.StudentChange.StudentOut();

        public List<System.Web.Mvc.SelectListItem> StudentChangeTypeList { get; set; } = new List<System.Web.Mvc.SelectListItem>();
    }
}