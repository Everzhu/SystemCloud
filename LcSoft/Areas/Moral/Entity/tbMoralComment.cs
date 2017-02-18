using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Moral.Entity
{
    /// <summary>
    /// 班主任评语
    /// </summary>
    public class tbMoralComment : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 学段
        /// </summary>
        [Required]
        [Display(Name = "学段")]
        public virtual Basis.Entity.tbYear tbYear { get; set; }

        /// <summary>
        /// 学生
        /// </summary>
        [Required]
        [Display(Name = "学生")]
        public virtual Student.Entity.tbStudent tbStudent { get; set; }

        /// <summary>
        /// 提交时间
        /// </summary>
        [Required]
        [Display(Name = "提交时间")]
        public DateTime InputDate { get; set; }

        /// <summary>
        /// 提交月份
        /// </summary>
        [Required]
        [Display(Name = "提交月份")]
        public string InputMonth { get; set; }

        /// <summary>
        /// 提交人员
        /// </summary>
        [Required]
        [Display(Name = "提交人员")]
        public Sys.Entity.tbSysUser tbSysUser { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        [Required]
        [Display(Name = "内容")]
        public string Comment { get; set; }
    }
}