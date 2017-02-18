using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Exam.Models.ExamLevel
{
    public class Edit
    {
        public Entity.tbExamLevel ExamLevelEdit { get; set; } = new Entity.tbExamLevel();

        public string LevelGroupId { get; set; }
    }
}