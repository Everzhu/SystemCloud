using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Moral.Entity
{
    /// <summary>
    /// 德育设定
    /// </summary>
    public class tbMoral : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 德育名称
        /// </summary>
        [Required]
        [Display(Name = "德育名称")]
        public string MoralName { get; set; }

        /// <summary>
        /// 学年
        /// </summary>
        [Required]
        [Display(Name = "学年")]
        public virtual Basis.Entity.tbYear tbYear { get; set; }

        /// <summary>
        /// 评价方式（每天/一次性）
        /// </summary>
        [Required]
        [Display(Name = "评价方式")]
        public Code.EnumHelper.MoralType MoralType { get; set; }

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

        /// <summary>
        /// 是否开放
        /// </summary>
        [Required]
        [Display(Name = "是否开放")]
        public bool IsOpen { get; set; }
    }
}