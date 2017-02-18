using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Basis.Entity
{
    /// <summary>
    /// 行政班学生
    /// </summary>
    public class tbClassStudent : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 行政班
        /// </summary>
        [Required]
        [Display(Name = "行政班")]
        public virtual tbClass tbClass { get; set; }

        /// <summary>
        /// 行政班小组
        /// </summary>
        [Display(Name = "行政班小组")]
        public virtual tbClassGroup tbClassGroup { get; set; }

        /// <summary>
        /// 行政班学生
        /// </summary>
        [Required]
        [Display(Name = "行政班学生")]
        public virtual Student.Entity.tbStudent tbStudent { get; set; }
    }
}