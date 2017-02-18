using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Dorm.Entity
{
    /// <summary>
    /// 住宿
    /// </summary>
    public class tbDorm : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 住宿名称
        /// </summary>
        [Required]
        [Display(Name = "住宿名称")]
        public string DormName { get; set; }

        /// <summary>
        /// 学年
        /// </summary>
        [Required]
        [Display(Name = "学年")]
        public virtual Basis.Entity.tbYear tbYear { get; set; }

        /// <summary>
        /// 开放申请
        /// </summary>
        [Required]
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