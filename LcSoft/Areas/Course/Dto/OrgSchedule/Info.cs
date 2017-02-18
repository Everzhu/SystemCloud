using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Course.Dto.OrgSchedule
{
    public class Info
    {
        /// <summary>
        /// 行政班
        /// </summary>
        public int ClassId { get; set; }

        /// <summary>
        /// 行政班级名称
        /// </summary>
        public string ClassName { get; set; }

        /// <summary>
        /// 科目ID
        /// </summary>
        public int SubjectId { get; set; }
        /// <summary>
        /// 课程
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// 课程名称
        /// </summary>
        public string CourseName { get; set; }
        /// <summary>
        /// 课程Id
        /// </summary>
        public int CourseId { get; set; }

        /// <summary>
        /// 星期
        /// </summary>
        public int WeekId { get; set; }

        /// <summary>
        /// 节次
        /// </summary>
        public int PeriodId { get; set; }

        /// <summary>
        /// 教师ID
        /// </summary>
        public int TeacherId { get; set; }

        /// <summary>
        /// 教师姓名
        /// </summary>
        public string TeacherName { get; set; }

        /// <summary>
        /// 教室
        /// </summary>
        public int RoomId { get; set; }

        /// <summary>
        /// 教室名称
        /// </summary>
        public string RoomName { get; set; }

        /// <summary>
        /// 学生
        /// </summary>
        public int StudentId { get; set; }

        /// <summary>
        /// 教学班ID
        /// </summary>
        public int OrgId { get; set; }

        /// <summary>
        /// 教学班名称
        /// </summary>
        public string OrgName { get; set; }

        /// <summary>
        /// 单双周
        /// </summary>
        public string ScheduleTypeName { get; set; }

        /// <summary>
        /// 单双周
        /// </summary>
        public Code.EnumHelper.CourseScheduleType ScheduleType { get; set; }
    }
}