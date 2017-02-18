using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Exam.Dto.ExamSchedule
{
    public class Import
    {
        public int Id { get; set; }

        /// <summary>
        /// 场次名称
        /// </summary>
        [Display(Name = "场次名称")]
        public string ScheduleName { get; set; }

        /// <summary>
        /// 考场名称
        /// </summary>
        [Display(Name = "考场名称")]
        public string ExamRoomName { get; set; }

        /// <summary>
        /// 考试课程
        /// </summary>
        [Display(Name = "考试课程")]
        public string ExamCourseName { get; set; }

        /// <summary>
        /// 考试教室
        /// </summary>
        [Display(Name = "考试教室")]
        public string RoomName { get; set; }

        /// <summary>
        /// 监考教师
        /// </summary>
        [Display(Name = "监考教师")]
        public string TeacherName { get; set; }

        [Display(Name = "教师工号")]
        public string TeacherCode { get; set; }

        /// <summary>
        /// 学生姓名
        /// </summary>
        [Display(Name = "学生姓名")]
        public string StudentName { get; set; }

        /// <summary>
        /// 学号
        /// </summary>
        [Display(Name = "学生学号")]
        public string StudentCode { get; set; }

        /// <summary>
        /// 导入提示
        /// </summary>
        [Display(Name = "导入提示")]
        public string Error { get; set; }
    }
}