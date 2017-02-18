using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Study.Dto.StudyApply
{
    public class CheckEdit
    {
        public int Id { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        [Display(Name = "排序")]
        public int? No { get; set; }
        /// <summary>
        /// 申请备注
        /// </summary>
        [Display(Name = "申请备注")]
        public string Remark { get; set; }
        /// <summary>
        /// 审批状态，0未审批，-1审批通过，1审批不通过
        /// </summary>
        [Display(Name = "审批状态")]
        public string CheckStatusName
        {
            get
            {
                return CheckStatus.GetDescription();
            }
        }
        /// <summary>
        /// 审批状态
        /// </summary>
        [Display(Name = "审批状态"), Required]
        public XkSystem.Code.EnumHelper.CheckStatus CheckStatus { get; set; }
        /// <summary>
        /// 审批说明
        /// </summary>
        [Display(Name = "审批说明"), Required]
        public string CheckRemark { get; set; }
    }
}