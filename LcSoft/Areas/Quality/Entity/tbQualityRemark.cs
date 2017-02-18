using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Quality.Entity
{
    /// <summary>
    /// 任课教师评语
    /// </summary>
    public class tbQualityRemark : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 学生
        /// </summary>
        [Required]
        [Display(Name = "学生")]
        public virtual Student.Entity.tbStudent tbStudent { get; set; }

        /// <summary>
        /// 教学班
        /// </summary>
        [Required]
        [Display(Name = "教学班")]
        public virtual Course.Entity.tbOrg tbOrg { get; set; }

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
        public Sys.Entity.tbSysUser tbSysUser { get; set; }

        /// <summary>
        /// 评语
        /// </summary>
        [Required]
        [Display(Name = "评语")]
        public string Remark { get; set; }
    }
}