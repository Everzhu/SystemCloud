using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XkSystem.Areas.Wechat.Entity
{
    /// <summary>
    /// 流程明细
    /// </summary>
    public class tbWeOAFlowDetail : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 流程类型
        /// </summary>
        [Required]
        [Display(Name = "流程类型")]
        public virtual tbWeOAFlowType tbWeOAFlowType { get; set; }

        /// <summary>
        /// 审批对象ID，对应审批内容表的ID,tbWeOAOffice,tbWeOACar...
        /// </summary>
        [Required]
        [Display(Name = "审批对象ID")]
        public int ApproveBodyId { get; set; }

        /// <summary>
        /// 审批意见
        /// </summary>
        //[Required]
        [Display(Name = "审批意见")]
        public string ApproveOpinion { get; set; }

        /// <summary>
        /// 审批人，第一个节点则为流程发起人
        /// </summary>
        [Required]
        [Display(Name = "审批人")]
        public Sys.Entity.tbSysUser tbSysUser { get; set; }

        /// <summary>
        /// 审批时间，第一个节点则为流程发起时间
        /// </summary>
        [Display(Name = "审批时间")]
        [Required]
        public DateTime ApproveDate { get; set; }

        /// <summary>
        /// 上个流程节点
        /// </summary>
        [Display(Name = "上个流程节点")]
        public tbWeOAFlowNode tbWeOAFlowPreviousNode { get; set; }

        /// <summary>
        /// 下个流程节点
        /// </summary>
        [Display(Name = "下个流程节点")]
        public tbWeOAFlowNode tbWeOAFlowNextNode { get; set; }

        /// <summary>
        /// 流程节点审批状态
        /// </summary>
        [Display(Name = "流程节点审批状态")]
        public Code.EnumHelper.OAFlowNodeStatus NodeApproveStatus { get; set; }

        /// <summary>
        /// 下个指定的审批人
        /// </summary>
        [Display(Name = "下个指定的审批人")]
        public virtual Sys.Entity.tbSysUser AssignNextApproveUser { get; set; }

    }
}
