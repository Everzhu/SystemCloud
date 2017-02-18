using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Elective.Entity
{
    /// <summary>
    /// 选课分段
    /// </summary>
    public class tbElectiveSection : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 分段名称
        /// </summary>
        [Required]
        [Display(Name = "分段名称")]
        public string ElectiveSectionName { get; set; }

        /// <summary>
        /// 所属选课
        /// </summary>
        [Required]
        [Display(Name = "所属选课")]
        public virtual tbElective tbElective { get; set; }

        /// <summary>
        /// 最大选课数
        /// </summary>
        [Required]
        [Display(Name = "最大选课数")]
        public int MaxElective { get; set; }

        /// <summary>
        /// 最小选课数
        /// </summary>
        [Required]
        [Display(Name = "最小选课数")]
        public int MinElective { get; set; }
    }
}