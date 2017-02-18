using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Student.Models.Student
{
    public class SelectStudent
    {
        public List<Dto.Student.SelectStudent> StudentList { get; set; } = new List<Dto.Student.SelectStudent>();

        public List<SelectListItem> GradeList { get; set; } = new List<SelectListItem>();

        public List<SelectListItem> ClassList { get; set; } = new List<SelectListItem>();

        public Code.PageHelper Page { get; set; } = new Code.PageHelper();

        public int? GradeId { get; set; } = System.Web.HttpContext.Current.Request["GradeId"].ConvertToInt();

        public int? ClassId { get; set; } = System.Web.HttpContext.Current.Request["ClassId"].ConvertToInt();

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();
    }
}