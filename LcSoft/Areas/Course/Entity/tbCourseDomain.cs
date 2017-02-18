using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Course.Entity
{
    /// <summary>
    /// 课程领域
    /// </summary>
    public class tbCourseDomain : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 课程领域
        /// </summary>
        [Required]
        [Display(Name = "课程领域")]
        public string CourseDomainName { get; set; }
    }
}