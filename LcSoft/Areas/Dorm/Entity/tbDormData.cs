using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Dorm.Entity
{
    /// <summary>
    /// 住宿表现记录
    /// </summary>
    public class tbDormData : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 学生
        /// </summary>
        [Required]
        [Display(Name = "学生")]
        public virtual Student.Entity.tbStudent tbStudent { get; set; }

        /// <summary>
        /// 住宿表现
        /// </summary>
        [Required]
        [Display(Name = "住宿表现")]
        public virtual tbDormOption tbDormOption { get; set; }

        /// <summary>
        /// 录入时间
        /// </summary>
        [Required]
        [Display(Name = "录入时间")]
        public DateTime InputDate { get; set; }

        /// <summary>
        /// 录入人员
        /// </summary>
        [Required]
        [Display(Name = "录入人员")]
        public virtual Sys.Entity.tbSysUser tbSysUser { get; set; }

        /// <summary>
        /// 备注说明
        /// </summary>
        [Display(Name = "备注说明")]
        public string Remark { get; set; }
    }
}