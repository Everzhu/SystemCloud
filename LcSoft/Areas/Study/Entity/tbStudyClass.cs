using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Study.Entity
{
    /// <summary>
    /// 晚自习班级
    /// </summary>
    public class tbStudyClass : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 晚自习
        /// </summary>
        [Display(Name = "晚自习"), Required]
        public virtual tbStudy tbStudy { get; set; }

        /// <summary>
        /// 班级
        /// </summary>
        [Display(Name = "班级"), Required]
        public virtual Basis.Entity.tbClass tbClass { get; set; }

        /// <summary>
        /// 晚自习教室
        /// </summary>
        [Display(Name = "晚自习教室")]
        public virtual Basis.Entity.tbRoom tbRoom { get; set; }
    }
}