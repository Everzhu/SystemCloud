using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Course.Entity
{
    /// <summary>
    /// 任课教师
    /// </summary>
    public class tbOrgTeacher : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 教学班
        /// </summary>
        [Required]
        [Display(Name = "教学班")]
        public virtual tbOrg tbOrg { get; set; }

        /// <summary>
        /// 任课教师
        /// </summary>
        [Required]
        [Display(Name = "任课教师")]
        public virtual Teacher.Entity.tbTeacher tbTeacher { get; set; }
    }
}