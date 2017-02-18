using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Course.Models.Course
{
    public class Edit
    {
        public Dto.Course.Edit CourseEdit { get; set; } = new Dto.Course.Edit();

        public List<System.Web.Mvc.SelectListItem> SubjectList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> CourseTypeList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> CourseDomainList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> CourseGroupList { get; set; } = new List<System.Web.Mvc.SelectListItem>();
    }
}