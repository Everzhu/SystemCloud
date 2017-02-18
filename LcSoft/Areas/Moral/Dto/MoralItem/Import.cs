using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Moral.Dto.MoralItem
{
    public class Import
    {
        [Display(Name ="项目名称")]
        public string MoralItemName { get; set; }

        [Display(Name ="德育分组")]
        public string MoralGroupName { get; set; }

        [Display(Name = "最小分值")]
        public string MinScore { get; set; }

        [Display(Name = "最大分值")]
        public string MaxScore { get; set; }

        [Display(Name = "初始分值")]
        public string InitScore { get; set; }

        [Display(Name = "选项类型")]
        public string MoralItemType { get; set; }

        [Display(Name = "表达式")]
        public string MoralExpress { get; set; }

        [Display(Name = "德育选项")]
        public string MoralOptionString { get; set; }

        [Display(Name = "导入结果")]
        public string ImportError { get; set; }

    }
}