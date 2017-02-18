using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Dorm.Dto.DormApply
{
    public class Approve
    {
        public int Id { get; set; }
        
        /// <summary>
        /// 学生姓名
        /// </summary>
        [Display(Name = "学生姓名")]
        public string StudentName { get; set; }

        /// <summary>
        /// 学生学号
        /// </summary>
        [Display(Name = "学生学号")]
        public string StudentCode { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        [Display(Name = "性别")]
        public string Sex { get; set; }

        /// <summary>
        /// 申请日期
        /// </summary>
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
        [Display(Name = "审批状态")]
        public Code.EnumHelper.CheckStatus CheckStatus { get; set; }

        ///// <summary>
        ///// 审批状态
        ///// </summary>
        //[Display(Name = "审批状态")]
        //public string CheckStatusName { get; set; }

        ///// <summary>
        ///// 审批时间
        ///// </summary>
        //[Display(Name = "审批时间")]
        //public DateTime CheckDate { get; set; }

        ///// <summary>
        ///// 审批人员编号
        ///// </summary>
        //[Display(Name = "审批人员编号")]
        //public string UserCode { get; set; }

        ///// <summary>
        ///// 审批人员姓名
        ///// </summary>
        //[Display(Name = "审批人员姓名")]
        //public string UserName { get; set; }

        /// <summary>
        /// 审批备注
        /// </summary>
        [Display(Name = "审批备注")]
        public string CheckRemark { get; set; }
    }
}