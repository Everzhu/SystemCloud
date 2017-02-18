using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Exam.Models.ExamSegmentGroup
{
    public class List
    {
        public List<Entity.tbExamSegmentGroup> ExamSegmentGroupList { get; set; } = new List<Entity.tbExamSegmentGroup>();

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();
    }
}