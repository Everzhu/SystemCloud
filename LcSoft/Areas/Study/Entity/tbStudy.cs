using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Study.Entity
{
    /// <summary>
    /// 晚自习
    /// </summary>
    public class tbStudy : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 晚自习名称
        /// </summary>
        [Display(Name = "晚自习名称"), Required]
        public string StudyName { get; set; }

        /// <summary>
        /// 学年
        /// </summary>
        [Display(Name = "学年"), Required]
        public virtual Basis.Entity.tbYear tbYear { get; set; }

        /// <summary>
        /// 晚自习模式（班级、教室）
        /// </summary>
        [Display(Name = "晚自习模式"), Required]
        public bool IsRoom { get; set; }

        /// <summary>
        /// 开放申请
        /// </summary>
        [Display(Name = "开放申请"), Required]
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