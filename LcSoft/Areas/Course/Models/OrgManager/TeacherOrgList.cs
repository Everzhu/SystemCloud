using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Course.Models.OrgManager
{
    public class TeacherOrgList
    {
        public List<Dto.OrgManager.TeacherOrgList> DataList { get; set; } = new List<Dto.OrgManager.TeacherOrgList>();

        public int TeacherId { get; set; } = HttpContext.Current.Request["TeacherId"].ConvertToInt();

        public string TeacherName { get; set; }

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();

        public Code.PageHelper Page { get; set; } = new Code.PageHelper();
    }
}