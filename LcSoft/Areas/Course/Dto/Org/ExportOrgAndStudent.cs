using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Course.Dto.Org
{
    public class ExportOrgAndStudent
    {
        /// <summary>
        /// 学生学号
        /// </summary>
        [Display(Name = "学生学号")]
        public string StudentCode { get; set; }

        /// <summary>
        /// 学生姓名
        /// </summary>
        [Display(Name = "学生姓名")]
        public string StudentName { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        [Display(Name = "性别")]
        public string SexName { get; set; }

        /// <summary>
        /// 教学班ID
        /// </summary>
        [Display(Name = "教学班ID")]
        public int OrgId { get; set; }

        /// <summary>
        /// 教学班
        /// </summary>
        [Display(Name = "教学班")]
        public string OrgName { get; set; }

        /// <summary>
        /// 年级
        /// </summary>
        [Display(Name = "年级")]
        public string GradeName { get; set; }

        public int? ClassId { get; set; }

        /// <summary>
        /// 班级名称（走班类型的教学班此字段为空）
        /// </summary>
        [Display(Name = "班级名称")]
        public string ClassName { get; set; }

        /// <summary>
        /// 课程
        /// </summary>
        [Display(Name = "课程")]
        public string CourseName { get; set; }

        /// <summary>
        /// 教室
        /// </summary>
        [Display(Name = "教室")]
        public string RoomName { get; set; }

        /// <summary>
        /// 教师工号
        /// </summary>
        [Display(Name = "教师工号")]
        public string TeacherCode { get; set; }

        /// <summary>
        /// 教师姓名
        /// </summary>
        [Display(Name = "教师姓名")]
        public string TeacherName { get; set; }

        /// <summary>
        /// 课表节次
        /// </summary>
        [Display(Name = "课表节次")]
        public string OrgSchedule { get; set; }
    }
}