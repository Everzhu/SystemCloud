using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Basis.Models.ClassTeacher
{
    public class List
    {
        public List<Dto.ClassTeacher.List> ClassTeacherList { get; set; } = new List<Dto.ClassTeacher.List>();

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();

        public int ClassId { get; set; }

        public string ClassTeacherId { get; set; }

        public string ClassTeacherName { get; set; }
    }
}