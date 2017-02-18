using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Perform.Entity
{
    /// <summary>
    /// 评价数据
    /// </summary>
    public class tbPerformData : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 评价课程
        /// </summary>
        [Required]
        [Display(Name = "评价课程")]
        public virtual Course.Entity.tbCourse tbCourse { get; set; }
        /// <summary>
        /// 评价学生
        /// </summary>
        [Required]
        [Display(Name = "评价学生")]
        public virtual Student.Entity.tbStudent tbStudent { get; set; }
        /// <summary>
        /// 评价项目
        /// </summary>
        [Required]
        [Display(Name = "评价项目")]
        public virtual tbPerformItem tbPerformItem { get; set; }
        /// <summary>
        /// 评价选项
        /// </summary>
        [Display(Name = "评价选项")]
        public virtual tbPerformOption tbPerformOption { get; set; }
        /// <summary>
        /// 评价分数
        /// </summary>
        [Required]
        [Display(Name = "评价分数")]
        public decimal Score { get; set; }
        /// <summary>
        /// 录入时间
        /// </summary>
        [Required]
        [Display(Name = "录入时间")]
        public DateTime InputDate { get; set; }
        /// <summary>
        /// 录入人员
        /// </summary>
        [Required]
        [Display(Name = "录入人员")]
        public virtual Sys.Entity.tbSysUser tbSysUser { get; set; }
    }
}