using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Basis.Entity
{
    /// <summary>
    /// 星期
    /// </summary>
    public class tbWeek : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 星期
        /// </summary>
        [Required]
        [Display(Name = "星期")]
        public string WeekName { get; set; }

        /// <summary>
        /// 星期编码
        /// </summary>
        [Required]
        [Display(Name = "星期编码")]
        public int WeekCode { get; set; }
    }
}