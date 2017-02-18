using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Moral.Entity
{
    /// <summary>
    /// 德育项目
    /// </summary>
    public class tbMoralItem : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 项目名称
        /// </summary>
        [Required]
        [Display(Name = "项目名称")]
        public string MoralItemName { get; set; }

        /// <summary>
        /// 所属德育
        /// </summary>
        [Required]
        [Display(Name = "所属德育")]
        public virtual tbMoralGroup tbMoralGroup { get; set; }

        /// <summary>
        /// 选项类型（下拉框、输入框）
        /// </summary>
        [Required]
        [Display(Name = "选项类型")]
        public Code.EnumHelper.MoralItemType MoralItemType { get; set; }

        /// <summary>
        /// 表达式
        /// </summary>
        [Required]
        [Display(Name = "表达式")]
        public Code.EnumHelper.MoralExpress MoralExpress { get; set; }

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

        /// <summary>
        /// 初始分值
        /// </summary>
        [Required]
        [Display(Name = "初始分值")]
        public decimal InitScore { get; set; }

        /// <summary>
        /// 评价类型
        /// </summary>
        [Required]
        [Display(Name = "评价类型")]
        public Code.EnumHelper.MoralItemKind MoralItemKind { get; set; }


        /// <summary>
        /// 操作方式
        /// </summary>
        [Required]
        [Display(Name = "操作方式")]
        public Code.EnumHelper.MoralItemOperateType MoralItemOperateType { get; set; }

        /// <summary>
        /// 初始分数
        /// </summary>
        [Required]
        [Display(Name = "初始分数")]
        public decimal DefaultValue { get; set; }

        /// <summary>
        /// 审核方式，默认自动
        /// </summary>
        [Required]
        [Display(Name = "审核方式")]
        public bool AutoCheck { get; set; } = true;

    }
}