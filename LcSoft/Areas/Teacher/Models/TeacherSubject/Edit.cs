using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Teacher.Models.TeacherSubject
{
    public class Edit
    {
        public List<System.Web.Mvc.SelectListItem> TeacherList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> GradeList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> SubjectList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<string> SelectedSubjectList { get; set; } = new List<string>();

        [Display(Name = "科研组长")]
        public int? TeacherId { get; set; } = System.Web.HttpContext.Current.Request["TeacherId"].ConvertToInt();
    }
}