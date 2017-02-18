using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Student.Dto.StudentChange
{
    public class StudentIn
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
        /// 性别
        /// </summary>
        [Display(Name = "性别"), Required]
        public string SexName { get; set; }

        /// <summary>
        /// 班级
        /// </summary>
        [Display(Name = "班级"), Required]
        public int ClassId { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [Display(Name = "备注")]
        public string Remark { get; set; }
    }
}