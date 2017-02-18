using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Course.Models.OrgStudent
{
    public class ListByTeacher
    {
        public List<Dto.OrgStudent.List> OrgStudentList { get; set; } = new List<Dto.OrgStudent.List>();

        public List<System.Web.Mvc.SelectListItem> OrgList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();

        public int OrgId { get; set; } = System.Web.HttpContext.Current.Request["OrgId"].ConvertToInt();

        /// <summary>
        /// 是否班主任：1.是
        /// </summary>
        public int IsClassTeacher { get; set; }= System.Web.HttpContext.Current.Request["IsClassTeacher"].ConvertToInt();
    }
}