using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Course.Models.OrgStudent
{
    public class OrgStudentList
    {
        public List<Dto.OrgStudent.OrgStudentList> List { get; set; } = new List<Dto.OrgStudent.OrgStudentList>();

        public Code.PageHelper Page { get; set; } = new Code.PageHelper();

        public List<System.Web.Mvc.SelectListItem> YearList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> GradeList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> SubjectList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();

        public int? YearId { get; set; } = System.Web.HttpContext.Current.Request["YearId"].ConvertToInt();

        public int? GradeId { get; set; } = System.Web.HttpContext.Current.Request["GradeId"].ConvertToInt();

        public int? SubjectId { get; set; } = System.Web.HttpContext.Current.Request["SubjectId"].ConvertToInt();
    }
}