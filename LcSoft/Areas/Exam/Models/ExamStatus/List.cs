using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Exam.Models.ExamStatus
{
    public class List
    {
        public List<Entity.tbExamStatus> ExamStatusList { get; set; } = new List<Entity.tbExamStatus>();

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();
    }
}