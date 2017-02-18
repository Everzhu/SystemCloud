using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XkSystem.Areas.Basis.Entity
{
    /// <summary>
    /// 班级管理员
    /// </summary>
    public class tbClassManager : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 行政班
        /// </summary>
        [Required]
        [Display(Name = "行政班")]
        public virtual tbClass tbClass { get; set; }

        /// <summary>
        /// 教师
        /// </summary>
        [Required]
        [Display(Name = "教师")]
        public virtual Teacher.Entity.tbTeacher tbTeacher { get; set; }
    }
}
