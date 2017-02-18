using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Basis.Entity
{
    /// <summary>
    /// 班主任
    /// </summary>
    public class tbClassTeacher : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 行政班
        /// </summary>
        [Required]
        [Display(Name = "行政班")]
        public virtual tbClass tbClass { get; set; }

        /// <summary>
        /// 班主任
        /// </summary>
        [Required]
        [Display(Name = "班主任")]
        public virtual Teacher.Entity.tbTeacher tbTeacher { get; set; }
    }
}