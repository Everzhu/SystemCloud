using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Quality.Entity
{
    public class tbQualityOption : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 评价内容
        /// </summary>
        [Required]
        [Display(Name = "评价内容")]
        public virtual tbQualityItem tbQualityItem { get; set; }

        /// <summary>
        /// 评价项
        /// </summary>
        [Required]
        [Display(Name = "评价项")]
        public string OptionName { get; set; }

        /// <summary>
        /// 评价分值
        /// </summary>
        [Required]
        [Display(Name = "评价分值")]
        public decimal OptionValue { get; set; }
    }
}