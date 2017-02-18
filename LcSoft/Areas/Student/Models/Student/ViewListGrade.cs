using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Student.Models.Student
{
    public class ViewListGrade
    {
        public List<Dto.Student.List> List { get; set; } = new List<Dto.Student.List>();

        public List<System.Web.Mvc.SelectListItem> GradeList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> ClassList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public bool IsGradeTeacher { get; set; }

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();

        public int GradeId { get; set; } = System.Web.HttpContext.Current.Request["GradeId"].ConvertToInt();

        public int? ClassId { get; set; } = System.Web.HttpContext.Current.Request["ClassId"].ConvertToInt();
    }
}