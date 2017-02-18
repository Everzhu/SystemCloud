using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Moral.Dto.MoralItem
{
    public class Edit
    {
        public int Id { get; set; }

        [Display(Name = "排序")]
        public int? No { get; set; }


        [Display(Name = "项目名称"), Required]
        public string MoralItemName { get; set; }

        [Display(Name = "德育分组"), Required]
        public int MoralGroupId { get; set; }

        /// <summary>
        /// 选项类型（下拉框、输入框）
        /// </summary>
        [Display(Name = "选项类型"), Required]
        public Code.EnumHelper.MoralItemType MoralItemType { get; set; }

        [Display(Name ="评价对象"),Required]
        public Code.EnumHelper.MoralItemKind MoralItemKind { get; set; }

        [Display(Name = "表达式"), Required]
        public Code.EnumHelper.MoralExpress MoralExpress { get; set; }

        [Display(Name = "操作方式"), Required]
        public Code.EnumHelper.MoralItemOperateType MoralItemOperateType { get; set; } = Code.EnumHelper.MoralItemOperateType.Score;

        [Display(Name = "最大分值"), Required]
        //^[0-9]+([.]{1}[0,5]){0,1}$
        [RegularExpression(Code.Common.RegIntAndDecimal, ErrorMessage = "分值只能为正整数或浮点数")]
        public decimal MaxScore { get; set; }

        [Display(Name = "最小分值"), Required]
        [RegularExpression(Code.Common.RegIntAndDecimal, ErrorMessage = "分值只能为正整数或浮点数")]
        public decimal MinScore { get; set; }

        [Display(Name = "初始分值"), Required]
        [RegularExpression(Code.Common.RegIntAndDecimal, ErrorMessage = "分值只能为正整数或浮点数")]
        public decimal InitScore { get; set; }

        [Display(Name = "基础分"), Required]
        [RegularExpression(Code.Common.RegIntAndDecimal, ErrorMessage = "分值只能为正整数或浮点数")]
        public decimal DefaultValue { get; set; }

        [Display(Name = "审核方式"), Required]
        public bool AutoCheck { get; set; } = true;
    }

}