using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Dict.Entity
{
    /// <summary>
    /// 专业
    /// </summary>
    public class tbDictMajor : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 专业名称
        /// </summary>
        [Display(Name = "专业名称"), Required]
        public string MajorName { get; set; }

        /// <summary>
        /// 专业代码
        /// </summary>
        [Display(Name = "专业代码"), Required]
        public string MajorCode { get; set; }
    }
}