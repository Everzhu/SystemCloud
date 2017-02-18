using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Course.Dto.Schedule
{
    public class Class
    {
        public int ClassId { get; set; }

        public string ClassName { get; set; }

        public int? ClassNo { get; set; }

        public List<ClassSchedule> ClassScheduleList { get; set; } = new List<ClassSchedule>();
    }

    public class ClassSchedule
    {
        public int CourseId { get; set; }

        public string CourseName { get; set; }

        public Code.EnumHelper.CourseScheduleType ScheduleType { get; set; } = Code.EnumHelper.CourseScheduleType.All;

        public int TeacherId { get; set; }

        public string TeacherName { get; set; }

        public int WeekId { get; set; }

        public string WeekName { get; set; }

        public int PeriodId { get; set; }

        public string PeriodName { get; set; }
    }
}