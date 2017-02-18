using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Student.Models.Student
{
    public class List
    {
        public List<Dto.Student.List> StudentList { get; set; } = new List<Dto.Student.List>();

        public List<System.Web.Mvc.SelectListItem> StudentTypeList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> StudentStudyTypeList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> StudentSessionList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> GradeList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> ClassList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public Code.PageHelper Page { get; set; } = new Code.PageHelper();

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();

        public int? StudentTypeId { get; set; } = System.Web.HttpContext.Current.Request["StudentTypeId"].ConvertToInt();

        public int? StudyTypeId { get; set; } = System.Web.HttpContext.Current.Request["StudyTypeId"].ConvertToInt();

        public int? StudentSessionId { get; set; } = System.Web.HttpContext.Current.Request["StudentSessionId"].ConvertToInt();

        public int? GradeId { get; set; } = System.Web.HttpContext.Current.Request["GradeId"].ConvertToInt();

        public int? ClassId { get; set; } = System.Web.HttpContext.Current.Request["ClassId"].ConvertToInt();
        
        public Code.EnumHelper.SysUserType UserType { get; set; } = Code.Common.UserType;
    }
}