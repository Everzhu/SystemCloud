using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Elective.Dto.ElectiveReport
{
    /// <summary>
    /// 选课学生列表
    /// </summary>
    public class Student
    {
        /// <summary>
        /// StudentId
        /// </summary>
        public int StudentId { get; set; }

        /// <summary>
        /// 学生姓名
        /// </summary>
        [Display(Name ="学生姓名")]
        public string StudentName { get; set; }

        /// <summary>
        /// 学生学号
        /// </summary>
        [Display(Name = "学生学号")]
        public string StudentCode { get; set; }

        /// <summary>
        /// 班级名称
        /// </summary>
        [Display(Name = "班级名称")]
        public string ClassName { get; set; }

        /// <summary>
        /// 选课时间
        /// </summary>
        [Display(Name = "选课时间")]
        public DateTime InputDate { get; set; }
    }
}