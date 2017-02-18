using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Perform.Dto.PerformItem
{
    public class Import
    {
        /// <summary>
        /// 排序
        /// </summary>
        [Display(Name = "排序")]
        public string No { get; set; }

        /// <summary>
        /// 评价项目
        /// </summary>
        [Display(Name = "评价项目")]
        public string PerformItemName { get; set; }

        /// <summary>
        /// 评价分组
        /// </summary>
        [Display(Name = "评价分组")]
        public string PerformGroupName { get; set; }

        /// <summary>
        /// 满分值
        /// </summary>
        [Display(Name = "满分值")]
        public string ScoreMax { get; set; }

        /// <summary>
        /// 折算比例
        /// </summary>
        [Display(Name = "折算比例")]
        public string Rate { get; set; }
        /// <summary>
        /// 是否选项
        /// </summary>
        [Display(Name = "是否选项")]
        public string IsSelect { get; set; }
        /// <summary>
        /// 是否多次
        /// </summary>
        [Display(Name = "是否多次")]
        public string IsMany { get; set; }
        /// <summary>
        /// 默认分数
        /// </summary>
        [Display(Name = "默认分数")]
        public string DefaultValue { get; set; }
        /// <summary>
        /// 选项排序
        /// </summary>
        [Display(Name = "选项排序")]
        public string OptionNo { get; set; }
        /// <summary>
        /// 选项名称
        /// </summary>
        [Display(Name = "选项名称")]
        public string OptionName { get; set; }
        /// <summary>
        /// 选项分值
        /// </summary>
        [Display(Name = "选项分值")]
        public string OptionValue { get; set; }
        /// <summary>
        /// 导入提示
        /// </summary>
        [Display(Name = "导入提示")]
        public string Error { get; set; }
    }
}