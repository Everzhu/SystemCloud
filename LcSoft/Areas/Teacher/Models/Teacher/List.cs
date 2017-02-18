using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Teacher.Models.Teacher
{
    public class List
    {
        public List<Dto.Teacher.List> TeacherList { get; set; } = new List<Dto.Teacher.List>();

        public Code.PageHelper Page { get; set; } = new Code.PageHelper();

        public List<System.Web.Mvc.SelectListItem> TeacherDeptList = new List<System.Web.Mvc.SelectListItem>();

        public int? TeacherDeptId { get; set; } = HttpContext.Current.Request["TeacherDeptId"].ConvertToInt();

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();
    }
}