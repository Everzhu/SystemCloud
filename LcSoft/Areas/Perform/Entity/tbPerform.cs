using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Perform.Entity
{
    /// <summary>
    /// 学生评价
    /// </summary>
    public class tbPerform : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 评价名称
        /// </summary>
        [Required]
        [Display(Name = "评价名称")]
        public string PerformName { get; set; }

        /// <summary>
        /// 学年
        /// </summary>
        [Required]
        [Display(Name = "学年")]
        public virtual Basis.Entity.tbYear tbYear { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        [Required]
        [Display(Name = "状态")]
        public bool IsOpen { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        [Required]
        [Display(Name = "开始时间")]
        public DateTime FromDate { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        [Required]
        [Display(Name = "结束时间")]
        public DateTime ToDate { get; set; }
    }
}