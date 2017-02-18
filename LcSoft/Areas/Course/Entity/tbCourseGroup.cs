using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XkSystem.Areas.Course.Entity
{
    /// <summary>
    /// 课程分组
    /// </summary>
    public class tbCourseGroup  : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 课程分组
        /// </summary>
        [Required]
        [Display(Name = "课程分组")]
        public string CourseGroupName { get; set; }
    }
}
