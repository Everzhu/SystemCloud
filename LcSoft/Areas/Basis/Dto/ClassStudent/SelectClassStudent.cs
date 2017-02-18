using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Basis.Dto.ClassStudent
{
    public class SelectClassStudent
    {
        /// <summary>
        /// 班级Id
        /// </summary>
        [Display(Name = "班级Id")]
        public int ClassId { get; set; }

        /// <summary>
        /// 学生Id
        /// </summary>
        [Display(Name = "学生Id")]
        public int StudentId { get; set; }

        /// <summary>
        /// 学号
        /// </summary>
        [Display(Name = "学号")]
        public string StudentCode { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        [Display(Name = "姓名")]
        public string StudentName { get; set; }

        /// <summary>
        /// 用户Id
        /// </summary>
        [Display(Name = "用户Id")]
        public int SysUserId { get; set; }
    }
}