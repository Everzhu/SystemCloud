using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Dorm.Dto.Dorm
{
    public class List
    {
        public int Id { get; set; }

        /// <summary>
        /// 住宿名称
        /// </summary>
        [Display(Name = "住宿名称")]
        public string DormName { get; set; }

        /// <summary>
        /// 学年
        /// </summary>
        [Display(Name = "学年")]
        public string YearName { get; set; }

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

        /// <summary>
        /// 是否已经申请
        /// </summary>
        [Display(Name ="是否已经申请")]
        public bool IsAlreadyApply { get; set; }

        public int DormApplyId { get; set; }
    }
}