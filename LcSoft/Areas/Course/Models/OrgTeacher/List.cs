using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Course.Models.OrgTeacher
{
    public class List
    {
        public List<Dto.OrgTeacher.List> OrgTeacherList { get; set; } = new List<Dto.OrgTeacher.List>();

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();

        public int OrgId { get; set; }
    }
}