using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Quality.Entity
{
    /// <summary>
    /// 学期总结
    /// </summary>
    public class tbQualitySummary : Code.EntityHelper.EntityBase
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
        /// 提交时间
        /// </summary>
        [Required]
        [Display(Name = "提交时间")]
        public DateTime InputDate { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        [Required]
        [Display(Name = "内容")]
        public string Content { get; set; }
    }
}