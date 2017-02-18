using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace XkSystem.Areas.Basis.Dto.Period
{
    public class List
    {
        public int Id { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        [Display(Name = "排序")]
        public int? No { get; set; }

        /// <summary>
        /// 节次
        /// </summary>
        [Display(Name = "节次")]
        public string PeriodName { get; set; }

        /// <summary>
        /// 颜色
        /// </summary>
        [Display(Name = "颜色")]
        public string Color { get; set; }
    }
}
