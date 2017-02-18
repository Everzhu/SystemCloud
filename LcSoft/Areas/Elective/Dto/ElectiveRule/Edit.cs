using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Elective.Dto.ElectiveRule
{
    public class Edit
    {
        /// <summary>
        /// CourseId
        /// </summary>
        public int CourseId { get; set; }

        /// <summary>
        /// 分段名称
        /// </summary>
        [Display(Name = "分段名称"), Required]
        public string CourseName { get; set; }

        /// <summary>
        /// 前置课程
        /// </summary>
        [Display(Name = "前置课程")]
        public string CourseTargetFront { get; set; }

        /// <summary>
        /// 互斥课程
        /// </summary>
        [Display(Name = "互斥课程")]
        public string CourseTargetExclue { get; set; }

        /// <summary>
        /// 后置课程
        /// </summary>
        [Display(Name = "后置课程")]
        public string CourseTargetBehind { get; set; }
    }
}