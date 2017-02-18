using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Wechat.Dto.ApplyCar
{
    public class ApplyCarEditDto
    {
        /// <summary>
        /// 用车部门
        /// </summary>
        [Display(Name = "用车部门ID")]
        [Required]
        public int tbTeacherDeptId { get; set; }

        /// <summary>
        /// 用车部门
        /// </summary>
        [Display(Name = "用车部门名称")]
        [Required]
        public string tbTeacherDeptName { get; set; }

        /// <summary>
        /// 目的地
        /// </summary>
        [Display(Name = "目的地")]
        [Required]
        public string Destination { get; set; }

        /// <summary>
        /// 用车原因
        /// </summary>
        [Display(Name = "用车原因")]
        public string Reason { get; set; }

        /// <summary>
        /// 用车时间
        /// </summary>
        [Display(Name = "用车时间")]
        [Required]
        public DateTime CarTime { get; set; }

        /// <summary>
        /// 随行人员
        /// </summary>
        [Display(Name = "随行人员")]
        public string OtherUsers { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [Display(Name = "备注")]
        public string Remark { get; set; }

        /// <summary>
        /// 指定审批人ID
        /// </summary>
        [Display(Name = "指定审批人")]
        [Required]
        public int NextApproveUserId { get; set; }

        /// <summary>
        /// 指定审批人名称
        /// </summary>
        [Display(Name = "指定审批人")]
        [Required]
        public string NextApproveUserName { get; set; }
    }
}