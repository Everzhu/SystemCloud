using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Perform.Entity
{
    /// <summary>
    /// 学生评语
    /// </summary>
    public class tbPerformComment : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 所属学生
        /// </summary>
        [Required]
        [Display(Name = "所属学生")]
        public virtual Student.Entity.tbStudent tbStudent { get; set; }

        /// <summary>
        /// 所属学年（取学期）
        /// </summary>
        [Required]
        [Display(Name = "所属学年")]
        public virtual Basis.Entity.tbYear tbYear { get; set; }

        /// <summary>
        /// 评语内容
        /// </summary>
        [Required]
        [Display(Name = "评语内容")]
        public string Comment { get; set; }

        /// <summary>
        /// 录入日期
        /// </summary>
        [Required]
        [Display(Name = "录入日期")]
        public DateTime InputDate { get; set; }

        /// <summary>
        /// 录入教师
        /// </summary>
        [Required]
        [Display(Name = "录入教师")]
        public virtual Sys.Entity.tbSysUser tbSysUser { get; set; }
    }
}