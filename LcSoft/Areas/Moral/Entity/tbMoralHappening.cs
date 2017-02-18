using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Moral.Entity
{
    /// <summary>
    /// 孩子家中情况
    /// </summary>
    public class tbMoralHappening : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 学年
        /// </summary>
        [Required]
        [Display(Name = "学年")]
        public virtual Basis.Entity.tbYear tbYear { get; set; }

        /// <summary>
        /// 学生
        /// </summary>
        [Required]
        [Display(Name = "学生")]
        public virtual Student.Entity.tbStudent tbStudent { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        [Required]
        [Display(Name = "内容")]
        public string Content { get; set; }

        /// <summary>
        /// 提交时间
        /// </summary>
        [Required]
        [Display(Name = "提交时间")]
        public DateTime InputDate { get; set; }

        /// <summary>
        /// 提交人员
        /// </summary>
        [Required]
        [Display(Name = "提交人员")]
        public virtual Sys.Entity.tbSysUser tbSysUser { get; set; }
    }
}