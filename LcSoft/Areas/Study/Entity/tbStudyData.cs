using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Study.Entity
{
    /// <summary>
    /// 晚自习分数
    /// </summary>
    public class tbStudyData : Code.EntityHelper.EntityBase
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
        /// 住宿表现
        /// </summary>
        [Display(Name = "住宿表现"), Required]
        public virtual tbStudyOption tbStudyOption { get; set; }

        /// <summary>
        /// 录入时间
        /// </summary>
        [Display(Name = "录入时间"), Required]
        public DateTime InputDate { get; set; }

        /// <summary>
        /// 录入人员
        /// </summary>
        [Display(Name = "录入人员"), Required]
        public virtual Sys.Entity.tbSysUser tbSysUser { get; set; }

        /// <summary>
        /// 备注说明
        /// </summary>
        [Display(Name = "备注说明")]
        public string Remark { get; set; }
    }
}