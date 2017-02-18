using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Basis.Entity
{
    public class tbPeriodType : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 节次类型
        /// </summary>
        [Required]
        [Display(Name = "节次类型")]
        public string PeriodTypeName { get; set; }

        /// <summary>
        /// 颜色
        /// </summary>
        [Required]
        [Display(Name = "颜色")]
        public string Color { get; set; }
    }
}