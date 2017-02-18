using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Study.Entity
{
    /// <summary>
    /// 晚自习申请
    /// </summary>
    public class tbStudyApply : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 晚自习
        /// </summary>
        [Display(Name = "晚自习"), Required]
        public virtual tbStudy tbStudy { get; set; }

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
        /// 备注说明
        /// </summary>
        [Display(Name = "备注说明")]
        public string Remark { get; set; }

        /// <summary>
        /// 审批状态
        /// </summary>
        [Display(Name = "审批状态"), Required]
        public Code.EnumHelper.CheckStatus CheckStatus { get; set; }

        /// <summary>
        /// 审批日期
        /// </summary>
        [Display(Name = "审批日期")]
        public DateTime? CheckDate { get; set; }

        /// <summary>
        /// 审批人员
        /// </summary>
        [Display(Name = "审批人员")]
        public virtual Sys.Entity.tbSysUser tbCheckUser { get; set; }

        /// <summary>
        /// 备注说明
        /// </summary>
        [Display(Name = "备注说明")]
        public string CheckRemark { get; set; }
    }
}