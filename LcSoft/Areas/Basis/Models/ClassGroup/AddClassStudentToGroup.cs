using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Basis.Models.ClassGroup
{
    public class AddClassStudentToGroup
    {
        public List<Dto.ClassGroup.AddClassStudentToGroup> ClassStudentList { get; set; } = new List<Dto.ClassGroup.AddClassStudentToGroup>();

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();

        public Code.PageHelper Page { get; set; } = new Code.PageHelper();

        public int ClassId { get; set; }

        public int ClassGroupId { get; set; }
    }
}