using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Basis.Entity
{
    /// <summary>
    /// 班级类别(行政班)
    /// </summary>
    public class tbClassType : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 班级类型
        /// </summary>
        [Required]
        [Display(Name = "班级类型")]
        public string ClassTypeName { get; set; }
    }
}