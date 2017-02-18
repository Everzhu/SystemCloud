using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Exam.Models.ExamLevel
{
    public class List
    {
        public List<Entity.tbExamLevel> ExamLevelList { get; set; } = new List<Entity.tbExamLevel>();

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();

        public int LevelGroupId { get; set; }
    }
}