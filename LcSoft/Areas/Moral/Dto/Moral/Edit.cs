using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Moral.Dto.Moral
{
    public class Edit
    {
        public int Id { get; set; }

        [Display(Name ="排序")]
        public int? No { get; set; }

        /// <summary>
        /// 德育名称
        /// </summary>
        [Display(Name = "德育名称"), Required]
        public string MoralName { get; set; }

        /// <summary>
        /// 学年
        /// </summary>
        [Display(Name = "学年"), Required]
        public int? tbYearId { get; set; }

        /// <summary>
        /// 评价方式（每天/一次性）
        /// </summary>
        [Display(Name = "评价方式"), Required]
        public Code.EnumHelper.MoralType MoralType { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        [Display(Name = "开始时间"), Required]
        public DateTime FromDate { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        [Display(Name ="结束时间"),Required]
        public DateTime ToDate { get; set; }

        /// <summary>
        /// 是否开放
        /// </summary>
        [Display(Name = "是否开放"), Required]
        public bool IsOpen { get; set; }
    }
}