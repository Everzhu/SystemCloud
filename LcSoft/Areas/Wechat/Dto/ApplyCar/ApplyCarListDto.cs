using XkSystem.Areas.Wechat.Dto.CommDto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Wechat.Dto.ApplyCar
{
    public class ApplyCarListDto : WeOAApproveFlowListDto
    {
        /// <summary>
        /// 标题
        /// </summary>
        public string Title
        {
            get
            {
                return ApplyUser + "的用车申请";
            }
        }

        /// <summary>
        /// 部门
        /// </summary>
        public string tbTeacherDeptName { get; set; }

        /// <summary>
        /// 目的地
        /// </summary>
        public string Destination { get; set; }

        /// <summary>
        /// 用车原因
        /// </summary>
        public string Reason { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 用车时间
        /// </summary>
        public DateTime CarTime { get; set; }

        /// <summary>
        /// 随行人员
        /// </summary>
        public string OtherUsers { get; set; }
    }
}