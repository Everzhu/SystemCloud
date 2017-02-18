using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Basis.Entity
{
    /// <summary>
    /// 节次
    /// </summary>
    public class tbPeriod : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 节次
        /// </summary>
        [Required]
        [Display(Name = "节次")]
        public string PeriodName { get; set; }

        /// <summary>
        /// 节次类型
        /// </summary>
        //[Required]
        [Display(Name = "节次类型")]
        public virtual tbPeriodType tbPeriodType { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        [Display(Name = "开始时间")]
        public string FromDate { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        [Display(Name = "结束时间")]
        public string ToDate { get; set; }
    }
}