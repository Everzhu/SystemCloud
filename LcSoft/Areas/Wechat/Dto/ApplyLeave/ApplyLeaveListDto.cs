using XkSystem.Areas.Wechat.Dto.CommDto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Wechat.Dto.ApplyLeave
{
    public class ApplyLeaveListDto : WeOAApproveFlowListDto
    {
        /// <summary>
        /// 标题
        /// </summary>
        public string Title
        {
            get
            {
                return ApplyUser + "的请假申请";
            }
        }

        /// <summary>
        /// 部门名称
        /// </summary>
        public string tbTeacherDeptName { get; set; }

        /// <summary>
        /// 请假类别
        /// </summary>
        public string tbWeOALeaveTypeName { get; set; }

        /// <summary>
        /// 请假事由
        /// </summary>
        public string Reason { get; set; }

        /// <summary>
        /// 请假开始时间
        /// </summary>
        public DateTime LeaveFromTime { get; set; }

        /// <summary>
        /// 请假结束时间
        /// </summary>
        public DateTime LeaveToTime { get; set; }

        /// <summary>
        /// 请假天数,0.5,1,1.5,2,2.5,3....
        /// </summary>
        public float LeaveDayCount { get; set; }

        /// <summary>
        /// 病例文件
        /// </summary>
        public string CaseFileName { get; set; }
    }
}