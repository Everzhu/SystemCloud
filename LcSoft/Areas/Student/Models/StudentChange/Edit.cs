using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Student.Models.StudentChange
{
    public class Edit
    {
        public Dto.StudentChange.Edit StudentChangeEdit { get; set; } = new Dto.StudentChange.Edit();

        public List<System.Web.Mvc.SelectListItem> StudentChangeTypeList { get; set; } = new List<System.Web.Mvc.SelectListItem>();
    }
}