using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Course.Dto.Schedule
{
    public class ClassSetImportWord
    {
        public int ClassId { get; set; }

        public string ClassName { get; set; }

        public int? ClassNo { get; set; }

        public List<ClassSetImportWordItem> ClassSetImportWordItemList { get; set; } = new List<ClassSetImportWordItem>();
    }

    public class ClassSetImportWordItem
    {
        public int CourseId { get; set; }

        public string CourseName { get; set; }

        public int TeacherId { get; set; }

        public string TeacherName { get; set; }

        public int WeekId { get; set; }

        public string WeekName { get; set; }

        public int PeriodId { get; set; }

        public string PeriodName { get; set; }
    }
}