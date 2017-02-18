using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Basis.Entity
{
    /// <summary>
    /// 行政班小组
    /// </summary>
    public class tbClassGroup : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 小组名称
        /// </summary>
        [Required]
        [Display(Name = "小组名称")]
        public string ClassGroupName { get; set; }
        
        /// <summary>
        /// 所属行政班
        /// </summary>
        [Required]
        [Display(Name = "所属行政班")]
        public virtual tbClass tbClass { get; set; }

        /// <summary>
        /// 导师
        /// </summary>
        [Required]
        [Display(Name = "导师")]
        public virtual Teacher.Entity.tbTeacher tbTeacher { get; set; }
    }
}