using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Survey.Entity
{
    /// <summary>
    /// 评价内容
    /// </summary>
    public class tbSurveyItem : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 评价分组
        /// </summary>
        [Display(Name = "评价分组"), Required]
        public virtual tbSurveyGroup tbSurveyGroup { get; set; }

        /// <summary>
        /// 评价内容
        /// </summary>
        [Display(Name = "评价内容"), Required]
        public string SurveyItemName { get; set; }

        /// <summary>
        /// 试题类型
        /// </summary>
        [Display(Name = "试题类型"), Required]
        public Code.EnumHelper.SurveyItemType SurveyItemType { get; set; }

        /// <summary>
        /// 是否纵向（排版布局）
        /// </summary>
        [Display(Name = "是否纵向"), Required]
        public bool IsVertical { get; set; }

        /// <summary>
        /// 字数限制
        /// </summary>
        [Display(Name = "字数限制")]
        public int TextMaxLength { get; set; }
    }
}