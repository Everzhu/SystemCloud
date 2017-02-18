using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Moral.Entity
{
    /// <summary>
    /// 德育选项
    /// </summary>
    public class tbMoralOption : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 选项名称
        /// </summary>
        [Required]
        [Display(Name = "选项名称")]
        public string MoralOptionName { get; set; }

        /// <summary>
        /// 分值
        /// </summary>
        [Required]
        [Display(Name = "分值")]
        public decimal MoralOptionValue { get; set; }

        /// <summary>
        /// 所属项目
        /// </summary>
        [Required]
        [Display(Name = "所属项目")]
        public virtual tbMoralItem tbMoralItem { get; set; }
    }
}