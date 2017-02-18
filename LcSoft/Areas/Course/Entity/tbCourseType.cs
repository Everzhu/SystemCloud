using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Course.Entity
{
    /// <summary>
    /// 课程类型
    /// </summary>
    public class tbCourseType : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 课程类型
        /// </summary>
        [Required]
        [Display(Name = "课程类型")]
        public string CourseTypeName { get; set; }
    }
}