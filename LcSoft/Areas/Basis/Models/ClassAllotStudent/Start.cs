using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Basis.Models.ClassAllotStudent
{
    public class Start
    {
        public List<Dto.ClassAllotStudent.Start> ClassAllotStudentList { get; set; } = new List<Dto.ClassAllotStudent.Start>();

        public int? YearId { get; set; } = System.Web.HttpContext.Current.Request["YearId"].ConvertToInt();

        public List<System.Web.Mvc.SelectListItem> YearList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public Code.PageHelper Page { get; set; } = new Code.PageHelper();

        public int? SexId { get; set; } = System.Web.HttpContext.Current.Request["SexId"].ConvertToInt();

        public int? ClassTypeId { get; set; } = System.Web.HttpContext.Current.Request["ClassTypeId"].ConvertToInt();

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();

        public List<System.Web.Mvc.SelectListItem> SexList = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> ClassTypeList = new List<System.Web.Mvc.SelectListItem>();
    }
}