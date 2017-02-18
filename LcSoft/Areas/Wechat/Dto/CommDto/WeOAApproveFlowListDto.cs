using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Wechat.Dto.CommDto
{
    public class WeOAApproveFlowListDto
    {
        #region 审批流程相关字段
        /// <summary>
        /// 审批内容标识符ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 申请人
        /// </summary>
        public string ApplyUser { get; set; }

        /// <summary>
        /// 审批意见
        /// </summary>
        public string ApproveOpinion { get; set; }

        /// <summary>
        /// 审批人ID
        /// </summary>
        public int ApproveUserId { get; set; }

        /// <summary>
        /// 审批人，第一个节点则为流程发起人
        /// </summary>
        public string ApproveUserName { get; set; }

        /// <summary>
        /// 审批时间，第一个节点则为流程发起时间
        /// </summary>
        public DateTime ApproveDate { get; set; }

        /// <summary>
        /// 审批节点
        /// </summary>
        public string FlowApprovalNode { get; set; }

        /// <summary>
        /// 流程步骤
        /// </summary>
        public int FlowStep { get; set; }

        /// <summary>
        /// 流程公式
        /// </summary>
        public string ConditionalFormula { get; set; }

        /// <summary>
        /// 流程节点审批状态
        /// </summary>
        public Code.EnumHelper.OAFlowNodeStatus NodeApproveStatus { get; set; }

        /// <summary>
        /// 节点状态
        /// </summary>
        public bool FlowComplete { get; set; }

        /// <summary>
        /// 是否审批完成
        /// </summary>
        public bool IsComplete { get; set; }
        #endregion
    }
}