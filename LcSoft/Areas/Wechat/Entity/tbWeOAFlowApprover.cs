using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XkSystem.Areas.Wechat.Entity
{
    /// <summary>
    /// 流程审批人
    /// </summary>
    public class tbWeOAFlowApprover : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 流程节点
        /// </summary>
        [Required]
        [Display(Name = "流程节点")]
        public virtual tbWeOAFlowNode tbWeOAFlowNode { get; set; }

        /// <summary>
        /// 流程节点审批人
        /// </summary>
        [Required]
        [Display(Name = "流程节点审批人")]
        public virtual Sys.Entity.tbSysUser tbSysUser { get; set; }
    }
}
