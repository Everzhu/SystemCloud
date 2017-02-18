using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Basis.Models.ClassAllotResult
{
    public class StartAllotClass
    {
        /// <summary>
        /// 分班方式
        /// </summary>
        [Display(Name = "分班方式")]
        public bool IsOrder { get; set; }

        /// <summary>
        /// 分数规则
        /// </summary>
        [Display(Name = "分数规则")]
        public bool IsScore { get; set; }

        public int YearId { get; set; } = System.Web.HttpContext.Current.Request["YearId"].ConvertToInt();
    }
}