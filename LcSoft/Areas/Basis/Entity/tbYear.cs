using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Basis.Entity
{
    /// <summary>
    /// 学年学段
    /// </summary>
    public class tbYear : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 学年学段
        /// </summary>
        [Required]
        [Display(Name = "学年学段")]
        public string YearName { get; set; }

        /// <summary>
        /// 上级学年
        /// </summary>
        [Display(Name = "上级学年")]
        public virtual tbYear tbYearParent { get; set; }

        /// <summary>
        /// 学年类型
        /// </summary>
        [Required]
        [Display(Name = "学年类型")]
        public Code.EnumHelper.YearType YearType { get; set; }

        /// <summary>
        /// 状态（开启/禁用)
        /// </summary>
        [Required]
        [Display(Name = "状态")]
        public bool IsDisable { get; set; }

        /// <summary>
        /// 是否默认
        /// </summary>
        [Required]
        [Display(Name = "是否默认")]
        public bool IsDefault { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        [Display(Name = "开始时间")]
        public DateTime? FromDate { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        [Display(Name = "结束时间")]
        public DateTime? ToDate { get; set; }
    }
}