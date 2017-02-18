using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Perform.Entity
{
    public class tbPerformOption : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 评价内容
        /// </summary>
        [Display(Name = "评价项目"), Required]
        public virtual tbPerformItem tbPerformItem { get; set; }
        /// <summary>
        /// 评价选项
        /// </summary>
        [Display(Name = "评价选项"), Required]
        public string OptionName { get; set; }
        /// <summary>
        /// 选项分值
        /// </summary>
        [Display(Name = "选项分值"), Required]
        public decimal OptionValue { get; set; }
    }
}