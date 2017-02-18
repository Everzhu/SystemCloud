using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Student.Models.Student
{
    public class EditStudent
    {
        public Dto.Student.EditStudent StudentEdit { get; set; } = new Dto.Student.EditStudent();

        public string Step { get; set; } = System.Web.HttpContext.Current.Request["Step"].ConvertToString();

        public List<System.Web.Mvc.SelectListItem> SexList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> StudentTypeList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> StudentSessionList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> StudentStudyTypeList { get; set; } = new List<System.Web.Mvc.SelectListItem>();
    }
}