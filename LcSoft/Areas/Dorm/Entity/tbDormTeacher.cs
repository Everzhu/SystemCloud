using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Dorm.Entity
{
    /// <summary>
    /// 宿管教师
    /// </summary>
    public class tbDormTeacher : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 教师
        /// </summary>
        [Required]
        [Display(Name = "教师")]
        public virtual Teacher.Entity.tbTeacher tbTeacher { get; set; }

        /// <summary>
        /// 管理寝室
        /// </summary>
        [Required]
        [Display(Name = "管理寝室")]
        public virtual Basis.Entity.tbRoom tbRoom { get; set; }
    }
}