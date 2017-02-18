using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Basis.Entity
{
    /// <summary>
    /// 行政班
    /// </summary>
    public class tbClass : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 行政班
        /// </summary>
        [Required]
        [Display(Name = "行政班")]
        public string ClassName { get; set; }

        /// <summary>
        /// 所属校区
        /// </summary>
        [Display(Name = "所属校区")]
        public virtual tbSchool tbSchool { get; set; }

        /// <summary>
        /// 学年
        /// </summary>
        [Required]
        [Display(Name = "学年")]
        public virtual tbYear tbYear { get; set; }

        /// <summary>
        /// 年级
        /// </summary>
        [Required]
        [Display(Name = "年级")]
        public virtual tbGrade tbGrade { get; set; }

        /// <summary>
        /// 班级类型
        /// </summary>
        [Required]
        [Display(Name = "班级类型")]
        public virtual tbClassType tbClassType { get; set; }

        /// <summary>
        /// 教室
        /// </summary>
        [Display(Name = "教室")]
        public virtual tbRoom tbRoom { get; set; }
    }
}