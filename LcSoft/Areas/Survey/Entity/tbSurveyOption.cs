using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Survey.Entity
{
    /// <summary>
    /// 评价选项
    /// </summary>
    public class tbSurveyOption : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 评价内容
        /// </summary>
        [Display(Name = "评价内容"), Required]
        public virtual tbSurveyItem tbSurveyItem { get; set; }

        /// <summary>
        /// 评价项
        /// </summary>
        [Display(Name = "评价项"), Required]
        public string OptionName { get; set; }

        /// <summary>
        /// 选项分值
        /// </summary>
        [Display(Name = "选项分值"), Required]
        public decimal OptionValue { get; set; }
    }
}