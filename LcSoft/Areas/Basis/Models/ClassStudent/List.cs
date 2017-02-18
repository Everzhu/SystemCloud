using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Basis.Models.ClassStudent
{
    public class List
    {
        public List<Dto.ClassStudent.List> ClassStudentList { get; set; } = new List<Dto.ClassStudent.List>();

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();

        public int ClassId { get; set; } = System.Web.HttpContext.Current.Request["ClassId"].ConvertToInt();

        public string ClassName { get; set; }

        public List<System.Web.Mvc.SelectListItem> ClassGroupList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public int? ClassGroupId { get; set; } = System.Web.HttpContext.Current.Request["ClassGroupId"].ConvertToInt();
    }
}