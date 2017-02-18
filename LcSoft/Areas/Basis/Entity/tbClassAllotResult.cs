using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Basis.Entity
{
    /// <summary>
    /// 分班结果
    /// </summary>
    public class tbClassAllotResult : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 分班班级
        /// </summary>
        [Display(Name = "分班班级")]
        public virtual tbClassAllotClass tbClassAllotClass { get; set; }

        /// <summary>
        /// 分班学生
        /// </summary>
        [Display(Name = "分班学生")]
        public virtual Student.Entity.tbStudent tbStudent { get; set; }

        /// <summary>
        /// 成绩
        /// </summary>
        [Display(Name = "成绩")]
        public decimal? Score { get; set; }
    }
}