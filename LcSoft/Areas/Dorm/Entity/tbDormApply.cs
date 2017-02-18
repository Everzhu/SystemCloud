using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Dorm.Entity
{
    /// <summary>
    /// 住宿申请
    /// </summary>
    public class tbDormApply : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 住宿
        /// </summary>
        [Required]
        [Display(Name = "住宿")]
        public virtual tbDorm tbDorm { get; set; }

        /// <summary>
        /// 申请学生
        /// </summary>
        [Required]
        [Display(Name = "申请学生")]
        public virtual Student.Entity.tbStudent tbStudent { get; set; }

        /// <summary>
        /// 申请日期
        /// </summary>
        [Required]
        [Display(Name = "申请日期")]
        public DateTime InputDate { get; set; }

        /// <summary>
        /// 备注说明
        /// </summary>
        [Display(Name = "备注说明")]
        public string Remark { get; set; }

        /// <summary>
        /// 审批状态
        /// </summary>
        [Required]
        [Display(Name = "审批状态")]
        public Code.EnumHelper.CheckStatus CheckStatus { get; set; }

        /// <summary>
        /// 审批时间
        /// </summary>
        [Display(Name = "审批时间")]
        public DateTime CheckDate { get; set; }

        /// <summary>
        /// 审批人员
        /// </summary>
        [Display(Name = "审批人员")]
        public virtual Sys.Entity.tbSysUser tbCheckUser { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [Display(Name = "备注")]
        public string CheckRemark { get; set; }
    }
}