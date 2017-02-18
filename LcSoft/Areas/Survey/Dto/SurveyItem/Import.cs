using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Survey.Dto.SurveyItem
{
    public class Import
    {
        /// <summary>
        /// 排序
        /// </summary>
        [Display(Name = "排序")]
        public string No { get; set; }

        /// <summary>
        /// 评教组
        /// </summary>
        [Display(Name = "评教组")]
        public string SurveyGroupName { get; set; }

        /// <summary>
        /// 评价内容
        /// </summary>
        [Display(Name = "评价内容")]
        public string SurveyItemName { get; set; }

        /// <summary>
        /// 选项布局
        /// </summary>
        [Display(Name = "选项布局")]
        public string IsVertical { get; set; }

        /// <summary>
        /// 试题类型
        /// </summary>
        [Display(Name = "试题类型")]
        public string SurveyItemType { get; set; }
        public List<ImportOption> ImportOptionList { get; set; }
        /// <summary>
        /// 导入提示
        /// </summary>
        [Display(Name = "导入提示")]
        public string Error { get; set; }
    }
    public class ImportOption
    {
        /// <summary>
        /// 评价内容
        /// </summary>
        [Display(Name = "评价内容")]
        public string SurveyColumnName { get; set; }
        /// <summary>
        /// 评价内容
        /// </summary>
        [Display(Name = "评价内容")]
        public string SurveyOptionName { get; set; }
        /// <summary>
        /// 评价内容
        /// </summary>
        [Display(Name = "评价分值")]
        public string SurveyOptionValue { get; set; }

    }
}