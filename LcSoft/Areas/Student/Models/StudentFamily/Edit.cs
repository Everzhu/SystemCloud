using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Student.Models.StudentFamily
{
    public class Edit
    {
        public Dto.StudentFamily.Edit StudentFamilyEdit { get; set; } = new Dto.StudentFamily.Edit();

        public List<System.Web.Mvc.SelectListItem> EducationList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> KinshipList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public int StudentId { get; set; }

        public bool Status { get; set; }
    }
}