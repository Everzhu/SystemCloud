using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Perform.Entity
{
    /// <summary>
    /// 评价项分组
    /// </summary>
    public class tbPerformGroup : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 分组名称
        /// </summary>
        [Required]
        [Display(Name = "分组名称")]
        public string PerformGroupName { get; set; }

        /// <summary>
        /// 评价
        /// </summary>
        [Required]
        [Display(Name = "评价")]
        public virtual tbPerform tbPerform { get; set; }

        /// <summary>
        /// 最大分值
        /// </summary>
        [Required]
        [Display(Name = "最大分值")]
        public decimal MaxScore { get; set; }

        /// <summary>
        /// 最小分值
        /// </summary>
        [Required]
        [Display(Name = "最小分值")]
        public decimal MinScore { get; set; }
    }
}