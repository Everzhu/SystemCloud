using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Dorm.Dto.Dorm
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
        /// 住宿名称
        /// </summary>
        [Display(Name = "住宿名称")]
        public string DormName { get; set; }

        /// <summary>
        /// 学年
        /// </summary>
        [Display(Name = "学年")]
        public int YearId { get; set; }

        /// <summary>
        /// 开放申请
        /// </summary>
        [Display(Name = "开放申请")]
        public bool IsApply { get; set; }

        /// <summary>
        /// 申请开始时间
        /// </summary>
        [Display(Name = "申请开始时间")]
        public DateTime ApplyFrom { get; set; }

        /// <summary>
        /// 申请结束时间
        /// </summary>
        [Display(Name = "申请结束时间")]
        public DateTime ApplyTo { get; set; }
    }
}