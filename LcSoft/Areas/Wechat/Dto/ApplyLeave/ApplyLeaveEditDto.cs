using XkSystem.Code;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Wechat.Dto.ApplyLeave
{
    public class ApplyLeaveEditDto
    {
        /// <summary>
        /// 部门
        /// </summary>
        [Display(Name = "部门")]
        [Required]
        public int tbTeacherDeptId { get; set; }

        /// <summary>
        /// 用车部门
        /// </summary>
        [Display(Name = "部门名称")]
        [Required]
        public string tbTeacherDeptName { get; set; }

        /// <summary>
        /// 请假类别
        /// </summary>
        [Display(Name = "请假类别")]
        [Required]
        public int tbWeOALeaveTypeId { get; set; }
        /// <summary>
        /// 请假类别
        /// </summary>
        [Display(Name = "请假类别")]
        [Required]
        public string tbWeOALeaveTypeName { get; set; }


        /// <summary>
        /// 请假事由
        /// </summary>
        [Display(Name = "请假事由")]
        [Required]
        public string Reason { get; set; }

        /// <summary>
        /// 请假开始时间
        /// </summary>
        [Display(Name = "开始时间")]
        [Required]
        public DateTime LeaveFromTime { get; set; }

        /// <summary>
        /// 请假结束时间
        /// </summary>
        [Display(Name = "结束时间")]
        [Required]
        [DateTimeNotLessThan("LeaveFromTime", "开始时间")]
        public DateTime LeaveToTime { get; set; }

        /// <summary>
        /// 请假天数,0.5,1,1.5,2,2.5,3....
        /// </summary>
        [Display(Name = "请假天数")]
        [Required]
        public float LeaveDayCount { get; set; }

        /// <summary>
        /// 病例文件
        /// </summary>
        [Display(Name = "病例文件")]
        public string CaseFileName { get; set; }

        /// <summary>
        /// 分支流程条件公式
        /// </summary>
        public string ConditionalFormula { get; set; }

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