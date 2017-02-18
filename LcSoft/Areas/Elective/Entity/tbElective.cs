using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Elective.Entity
{
    /// <summary>
    /// 选课设置
    /// </summary>
    public class tbElective : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 选课名称 
        /// </summary>
        [Required]
        [Display(Name = "选课名称")]
        public string ElectiveName { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        [Required]
        [Display(Name = "开始时间")]
        public DateTime FromDate { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        [Required]
        [Display(Name = "结束时间")]
        public DateTime ToDate { get; set; }

        [Display(Name ="申报开始时间"),Required]
        public DateTime ApplyFromDate { get; set; }

        [Display(Name = "申报结束时间"), Required]
        public DateTime ApplyToDate { get; set; }

        /// <summary>
        /// 选课模式
        /// </summary>
        [Required]
        [Display(Name = "选课模式")]
        public virtual tbElectiveType tbElectiveType { get; set; }

        /// <summary>
        /// 是否弹窗
        /// </summary>
        [Required]
        [Display(Name = "是否弹窗")]
        public bool IsPop { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        [Required]
        [Display(Name = "状态")]
        public bool IsDisable { get; set; }

        /// <summary>
        /// 选课说明
        /// </summary>
        [Display(Name = "选课说明")]
        public string Remark { get; set; }
    }
}