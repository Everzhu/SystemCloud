using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Quality.Entity
{
    /// <summary>
    /// 班主任评语
    /// </summary>
    public class tbQualityComment : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 学年
        /// </summary>
        [Display(Name = "学年"), Required]
        public virtual Basis.Entity.tbYear tbYear { get; set; }

        /// <summary>
        /// 学生
        /// </summary>
        [Display(Name = "学生"), Required]
        public virtual Student.Entity.tbStudent tbStudent { get; set; }

        /// <summary>
        /// 提交时间
        /// </summary>
        [Display(Name = "提交时间"), Required]
        public DateTime InputDate { get; set; }

        /// <summary>
        /// 提交人员
        /// </summary>
        [Display(Name = "提交人员"), Required]
        public Sys.Entity.tbSysUser tbSysUser { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        [Display(Name = "内容"), Required]
        public string Comment { get; set; }
    }
}