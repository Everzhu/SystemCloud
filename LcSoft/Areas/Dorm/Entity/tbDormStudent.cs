using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Dorm.Entity
{
    /// <summary>
    /// 住宿学生
    /// </summary>
    public class tbDormStudent : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 住宿
        /// </summary>
        [Required]
        [Display(Name = "住宿")]
        public virtual tbDorm tbDorm { get; set; }

        /// <summary>
        /// 寝室
        /// </summary>
        [Required]
        [Display(Name = "寝室")]
        public virtual Basis.Entity.tbRoom tbRoom { get; set; }

        /// <summary>
        /// 住宿学生
        /// </summary>
        [Required]
        [Display(Name = "住宿学生")]
        public virtual Student.Entity.tbStudent tbStudent { get; set; }
    }
}