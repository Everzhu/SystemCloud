using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Basis.Models.ClassStudent
{
    public class ClassStudentList
    {
        public List<Dto.ClassStudent.ClassStudentList> List { get; set; } = new List<Dto.ClassStudent.ClassStudentList>();
        
        public List<System.Web.Mvc.SelectListItem> YearList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> GradeList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> ClassTypeList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> ClassList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public Code.PageHelper Page { get; set; } = new Code.PageHelper();

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();

        public int? YearId { get; set; } = System.Web.HttpContext.Current.Request["YearId"].ConvertToInt();

        public int? GradeId { get; set; } = System.Web.HttpContext.Current.Request["GradeId"].ConvertToInt();

        public int? ClassTypeId { get; set; } = System.Web.HttpContext.Current.Request["ClassTypeId"].ConvertToInt();

        public int? ClassId { get; set; } = System.Web.HttpContext.Current.Request["ClassId"].ConvertToInt();
    }
}