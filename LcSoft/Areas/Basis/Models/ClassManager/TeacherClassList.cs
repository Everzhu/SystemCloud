using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Basis.Models.ClassManager
{
    public class TeacherClassList
    {
        public List<Dto.ClassManager.TeacherClassList> DataList { get; set; } = new List<Dto.ClassManager.TeacherClassList>();

        public int TeacherId { get; set; } = HttpContext.Current.Request["TeacherId"].ConvertToInt();

        public string TeacherName { get; set; }

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();

        public Code.PageHelper Page { get; set; } = new Code.PageHelper();
    }
}