using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Student.Entity
{
    /// <summary>
    /// 学生调动
    /// </summary>
    public class tbStudentChange : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 异动学生
        /// </summary>
        [Display(Name = "异动学生"), Required]
        public virtual tbStudent tbStudent { get; set; }

        /// <summary>
        /// 学生状态
        /// </summary>
        [Display(Name = "学生状态"), Required]
        public virtual tbStudentChangeType tbStudentChangeType { get; set; }

        /// <summary>
        /// 备注信息
        /// </summary>
        [Display(Name = "备注信息")]
        public string Remark { get; set; }

        /// <summary>
        /// 录入人员
        /// </summary>
        [Display(Name = "录入人员"), Required]
        public virtual Sys.Entity.tbSysUser tbSysUser { get; set; }

        /// <summary>
        /// 录入时间
        /// </summary>
        [Display(Name = "录入时间"), Required]
        public DateTime InputDate { get; set; }
    }
}