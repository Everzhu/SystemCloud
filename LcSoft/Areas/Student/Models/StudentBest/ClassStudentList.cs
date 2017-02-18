using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Student.Models.StudentBest
{
    public class ClassStudentList
    {
        public List<Dto.StudentBest.ClassStudentList> DataList { get; set; } = new List<Dto.StudentBest.ClassStudentList>();

        public int ClassId { get; set; } = HttpContext.Current.Request["ClassId"].ConvertToInt();

        public string SearchText { get; set; } = HttpContext.Current.Request["SearchText"].ConvertToString();
    }
}