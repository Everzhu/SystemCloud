using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Student.Models.StudentChangeType
{
    public class Edit
    {
        public Entity.tbStudentChangeType StudentChangeTypeEdit { get; set; } = new Entity.tbStudentChangeType();

        public List<System.Web.Mvc.SelectListItem> StudentChangeTypeList { get; set; } = new List<System.Web.Mvc.SelectListItem>();
    }
}