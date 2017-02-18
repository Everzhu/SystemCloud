using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Basis.Entity
{
    /// <summary>
    /// 分班学生
    /// </summary>
    public class tbClassAllotStudent : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 学年
        /// </summary>
        [Required]
        [Display(Name = "学年")]
        public virtual tbYear tbYear { get; set; }

        /// <summary>
        /// 分班学生
        /// </summary>
        [Required]
        [Display(Name = "分班学生")]
        public virtual Student.Entity.tbStudent tbStudent { get; set; }

        /// <summary>
        /// 分班年级
        /// </summary>
        [Required]
        [Display(Name = "分班年级")]
        public virtual tbGrade tbGrade { get; set; }

        /// <summary>
        /// 班级类型
        /// </summary>
        [Required]
        [Display(Name = "班级类型")]
        public virtual tbClassType tbClassType { get; set; }

        /// <summary>
        /// 分班成绩
        /// </summary>
        [Display(Name = "分班成绩")]
        public decimal? Score { get; set; }
    }
}