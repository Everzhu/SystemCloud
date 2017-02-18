using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace XkSystem.Areas.Basis.Dto.Period
{
    public class Edit
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
        [Display(Name = "节次"), Required]
        public string PeriodName { get; set; }

        /// <summary>
        /// 节次类型
        /// </summary>
        [Display(Name = "节次类型"), Required]
        public int PeriodTypeId { get; set; }
    }
}
