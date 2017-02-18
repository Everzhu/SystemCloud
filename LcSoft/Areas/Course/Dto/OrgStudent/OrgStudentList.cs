using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Course.Dto.OrgStudent
{
    public class OrgStudentList
    {
        public int Id { get; set; }

        /// <summary>
        /// 学生姓名
        /// </summary>
        [Display(Name = "学生姓名")]
        public string StudentName { get; set; }

        /// <summary>
        /// 学生学号
        /// </summary>
        [Display(Name = "学生学号")]
        public string StudentCode { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        [Display(Name = "性别")]
        public string SexName { get; set; }

        /// <summary>
        /// 座位号
        /// </summary>
        [Display(Name = "座位号")]
        public int? No { get; set; }

        /// <summary>
        /// 教学班
        /// </summary>
        [Display(Name = "教学班")]
        public string OrgName { get; set; }

        /// <summary>
        /// 教学班
        /// </summary>
        [Display(Name = "教学班")]
        public int OrgId { get; set; }

        public bool IsClass { get; set; }

        /// <summary>
        /// 绑定行政班
        /// </summary>
        [Display(Name = "绑定行政班")]
        public string ClassName { get; set; }

        /// <summary>
        /// 任课教师
        /// </summary>
        [Display(Name = "任课教师")]
        public string TeacherName { get; set; }
    }
}