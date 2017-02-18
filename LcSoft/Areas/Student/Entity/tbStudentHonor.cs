using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Student.Entity
{
    /// <summary>
    /// 学生获奖情况
    /// </summary>
    public class tbStudentHonor : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 所属学生
        /// </summary>
        [Display(Name = "所属学生"), Required]
        public virtual tbStudent tbStudent { get; set; }

        /// <summary>
        /// 荣誉名称
        /// </summary>
        [Display(Name = "荣誉名称"), Required]
        public string HonorName { get; set; }

        /// <summary>
        /// 学年
        /// </summary>
        [Display(Name = "学年")]
        public virtual Basis.Entity.tbYear tbYear { get; set; }

        /// <summary>
        /// 荣誉类型
        /// </summary>
        [Display(Name = "荣誉类型"), Required]
        public virtual tbStudentHonorType tbStudentHonorType { get; set; }

        /// <summary>
        /// 获奖级别
        /// </summary>
        [Display(Name = "获奖级别"), Required]
        public virtual tbStudentHonorLevel tbstudentHonorLevel { get; set; }

        /// <summary>
        /// 荣誉来源
        /// </summary>
        [Display(Name = "荣誉来源"), Required]
        public Code.EnumHelper.StudentHonorSource HonorSource { get; set; }

        /// <summary>
        /// 荣誉证书
        /// </summary>
        [Display(Name = "荣誉证书")]
        public string HonorFile { get; set; }

        /// <summary>
        /// 录入人员
        /// </summary>
        [Display(Name = "录入人员"), Required]
        public virtual Sys.Entity.tbSysUser tbUserInput { get; set; }

        /// <summary>
        /// 录入时间
        /// </summary>
        [Display(Name = "录入时间"), Required]
        public DateTime InputDate { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        [Display(Name = "状态"), Required]
        public Code.EnumHelper.CheckStatus CheckStatus { get; set; }

        /// <summary>
        /// 审批意见
        /// </summary>
        [Display(Name = "审批意见")]
        public string CheckRemark { get; set; }
    }
}