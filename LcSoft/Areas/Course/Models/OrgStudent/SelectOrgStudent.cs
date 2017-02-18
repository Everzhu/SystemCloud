using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Course.Models.OrgStudent
{
    public class SelectOrgStudent
    {
        public List<Dto.OrgStudent.SelectOrgStudent> OrgStudentList { get; set; } = new List<Dto.OrgStudent.SelectOrgStudent>();
        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();
    }
}