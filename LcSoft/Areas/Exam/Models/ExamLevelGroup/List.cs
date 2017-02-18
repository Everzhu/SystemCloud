using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Exam.Models.ExamLevelGroup
{
    public class List
    {
        public List<Entity.tbExamLevelGroup> ExamLevelGroupList { get; set; } = new List<Entity.tbExamLevelGroup>();

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();
    }
}