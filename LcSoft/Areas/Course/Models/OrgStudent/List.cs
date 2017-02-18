using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Course.Models.OrgStudent
{
    public class List
    {
        public List<Dto.OrgStudent.List> OrgStudentList { get; set; } = new List<Dto.OrgStudent.List>();

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();

        public int OrgId { get; set; } //= System.Web.HttpContext.Current.Request["OrgId"].ConvertToInt();

        public bool IsClass { get; set; }

        public string OrgName { get; set; }
    }
}