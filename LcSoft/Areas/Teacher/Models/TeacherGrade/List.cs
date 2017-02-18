using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Teacher.Models.TeacherGrade
{
    public class List
    {
        public List<Dto.TeacherGrade.List> TeacherGradeList { get; set; } = new List<Dto.TeacherGrade.List>();

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();

        public List<System.Web.Mvc.SelectListItem> GradeList { get; set; } = new List<System.Web.Mvc.SelectListItem>();
    }
}