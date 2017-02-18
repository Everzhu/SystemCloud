using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Exam.Entity
{
    /// <summary>
    /// 监考教师
    /// </summary>
    public class tbExamTeacher : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 考场
        /// </summary>
        [Required]
        [Display(Name = "考场")]
        public virtual tbExamRoom tbExamRoom { get; set; }

        /// <summary>
        /// 监考教师
        /// </summary>
        [Required]
        [Display(Name = "监考教师")]
        public virtual Teacher.Entity.tbTeacher tbTeacher { get; set; }

        /// <summary>
        /// 主监考
        /// </summary>
        [Required]
        [Display(Name = "主监考")]
        public bool IsPrimary { get; set; }
    }
}