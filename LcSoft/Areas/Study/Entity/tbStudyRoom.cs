using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Study.Entity
{
    /// <summary>
    /// 晚自习教室
    /// </summary>
    public class tbStudyRoom : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 晚自习
        /// </summary>
        [Display(Name = "晚自习"), Required]
        public virtual tbStudy tbStudy { get; set; }

        /// <summary>
        /// 晚自习教室
        /// </summary>
        [Display(Name = "晚自习教室"), Required]
        public virtual Basis.Entity.tbRoom tbRoom { get; set; }
    }
}