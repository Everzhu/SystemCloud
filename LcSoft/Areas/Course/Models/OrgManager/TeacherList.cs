using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Course.Models.OrgManager
{
    public class TeacherList
    {
        public List<Dto.OrgManager.TeacherList> DataList { get; set; } = new List<Dto.OrgManager.TeacherList>();

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();

        public Code.PageHelper Page { get; set; } = new Code.PageHelper();
    }
}