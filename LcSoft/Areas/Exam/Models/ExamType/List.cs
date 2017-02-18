using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Exam.Models.ExamType
{
    public class List
    {
        public List<Entity.tbExamType> ExamTypeList { get; set; } = new List<Entity.tbExamType>();

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();
    }
}