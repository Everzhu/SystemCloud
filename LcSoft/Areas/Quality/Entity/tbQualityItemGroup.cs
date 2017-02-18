using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Quality.Entity
{
    public class tbQualityItemGroup : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 评价分组
        /// </summary>
        [Required]
        [Display(Name = "评价分组")]
        public string QualityItemGroupName { get; set; }

        /// <summary>
        /// 评价
        /// </summary>
        [Required]
        [Display(Name = "评价")]
        public virtual tbQuality tbQuality { get; set; }
    }
}