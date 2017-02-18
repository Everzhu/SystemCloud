using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.ComponentModel.DataAnnotations;

namespace XkSystem.Areas.Teacher.Dto.Teacher
{
    public class Info
    {
        public int Id { get; set; }
        /// <summary>
        /// 教师Id
        /// </summary>
        [Display(Name = "教师Id")]
        public int TeacherId { get; set; }
        /// <summary>
        /// 教师姓名
        /// </summary>
        [Display(Name = "教师姓名")]
        public string TeacherName { get; set; }

        /// <summary>
        /// 教职工号
        /// </summary>
        [Display(Name = "教职工号")]
        public string TeacherCode { get; set; }

        /// <summary>
        /// 教师部门
        /// </summary>
        [Display(Name = "教师部门")]
        public string TeacherDeptName { get; set; }
    }
}