using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Student.Dto.StudentChange
{
    public class StudentReset
    {
        /// <summary>
        /// 学生学号
        /// </summary>
        [Display(Name = "学生学号"), Required]
        public string StudentCode { get; set; }

        /// <summary>
        /// 学生姓名
        /// </summary>
        [Display(Name = "学生姓名"), Required]
        public string StudentName { get; set; }

        /// <summary>
        /// 班级
        /// </summary>
        [Display(Name = "班级"), Required]
        public int ClassId { get; set; }

        /// <summary>
        /// 学届
        /// </summary>
        [Display(Name = "学届"), Required]
        public int StudentSessionId { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [Display(Name = "备注")]
        public string Remark { get; set; }
    }
}