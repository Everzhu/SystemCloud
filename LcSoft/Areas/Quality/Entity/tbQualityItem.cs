using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Quality.Entity
{
    public class tbQualityItem : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 评价分组
        /// </summary>
        [Required]
        [Display(Name = "评价分组")]
        public virtual tbQualityItemGroup tbQualityItemGroup { get; set; }

        /// <summary>
        /// 评价内容
        /// </summary>
        [Required]
        [Display(Name = "评价内容")]
        public string QualityItemName { get; set; }

        /// <summary>
        /// 题目类型
        /// </summary>
        [Required]
        [Display(Name = "题目类型")]
        public Code.EnumHelper.QualityItemType QualityItemType { get; set; }

        /// <summary>
        /// 是否纵向（排版布局）
        /// </summary>
        [Required]
        [Display(Name = "是否纵向")]
        public bool IsVertical { get; set; }
    }
}