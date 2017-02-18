using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Exam.Models.ExamLevelGroup
{
    public class Edit
    {
        public Entity.tbExamLevelGroup ExamLevelGroupEdit { get; set; } = new Entity.tbExamLevelGroup();

        public List<Entity.tbExamLevel> ExamLevelList { get; set; } = new List<Entity.tbExamLevel>();
    }
}