using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static XkSystem.Code.PageHelper;

namespace XkSystem.Areas.Moral.Dto.MoralItem
{
    public class List
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }

        [Display(Name ="序号")]
        public int? No { get; set; }

        [Display(Name = "项目名称")]
        public string MoralItemName { get; set; }

        [Display(Name = "德育分组")]
        public string MoralGroupName { get; set; }

        public int MoralGroupId { get; set; }

        /// <summary>
        /// 选项类型（下拉框、输入框）
        /// </summary>
        [Display(Name = "选项类型")]
        public Code.EnumHelper.MoralItemType MoralItemType { get; set; }


        /// <summary>
        /// 操作方式 打分、评语
        /// </summary>
        [Display(Name = "操作方式")]
        public Code.EnumHelper.MoralItemOperateType MoralItemOperateType { get; set; }

        [Display(Name = "表达式")]
        public Code.EnumHelper.MoralExpress MoralExpress { get; set; }

        [Display(Name ="评价对象")]
        public Code.EnumHelper.MoralItemKind MoralItemKind { get; set; }

        [Display(Name = "最大分值")]
        public decimal MaxScore { get; set; }

        [Display(Name = "最小分值")]
        public decimal MinScore { get; set; }

        [Display(Name = "初始分值")]
        public decimal InitScore { get; set; }

        [Display(Name ="基础分")]
        public Decimal DefaultValue { get; set; }

        [Display(Name ="审核方式")]
        public bool AutoCheck { get; set; }

    }
}