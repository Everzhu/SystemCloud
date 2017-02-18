using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Basis.Entity
{
    /// <summary>
    /// 年级
    /// </summary>
    public class tbGrade : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 年级
        /// </summary>
        [Required]
        [Display(Name = "年级")]
        public string GradeName { get; set; }

        /// <summary>
        /// 年级类型
        /// </summary>
        [Display(Name = "年级类型")]
        public virtual tbGradeType tbGradeType { get; set; }
    }
}