using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XkSystem.Areas.Basis.Dto.Year
{
    public class Edit
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        [Display(Name = "排序")]
        public int? No { get; set; }

        /// <summary>
        /// 学年
        /// </summary>
        [Display(Name = "学年"), Required]
        public string YearName { get; set; }

        /// <summary>
        /// 状态（开启/禁用)
        /// </summary>
        [Display(Name = "状态"), Required]
        public bool IsDisable { get; set; }

        /// <summary>
        /// 默认
        /// </summary>
        [Display(Name = "默认"), Required]
        public bool IsDefault { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        [Display(Name = "开始时间")]
        public DateTime? FromDate { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        [Display(Name = "结束时间")]
        public DateTime? ToDate { get; set; }

        public Code.EnumHelper.YearType YearTypeCode { get; set; }
    }
}
