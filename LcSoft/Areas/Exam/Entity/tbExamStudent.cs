using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Exam.Entity
{
    /// <summary>
    /// 考生
    /// </summary>
    public class tbExamStudent : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 考场
        /// </summary>
        [Required]
        [Display(Name = "考场")]
        public virtual tbExamRoom tbExamRoom { get; set; }

        /// <summary>
        /// 考生
        /// </summary>
        [Required]
        [Display(Name = "考生")]
        public virtual Student.Entity.tbStudent tbStudent { get; set; }
    }
}