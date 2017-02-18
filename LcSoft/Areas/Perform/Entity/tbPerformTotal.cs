using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Perform.Entity
{
    /// <summary>
    /// 评价汇总
    /// </summary>
    public class tbPerformTotal : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 对应评价
        /// </summary>
        [Required]
        [Display(Name = "对应评价")]
        public virtual tbPerform tbPerform { get; set; }

        /// <summary>
        /// 评价课程
        /// </summary>
        [Required]
        [Display(Name = "评价课程")]
        public virtual Course.Entity.tbCourse tbCourse { get; set; }

        /// <summary>
        /// 评价学生
        /// </summary>
        [Required]
        [Display(Name = "评价学生")]
        public virtual Student.Entity.tbStudent tbStudent { get; set; }

        /// <summary>
        /// 评价总分
        /// </summary>
        [Required]
        [Display(Name = "评价总分")]
        public decimal TotalScore { get; set; }
    }
}