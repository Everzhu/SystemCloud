using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XkSystem.Areas.Wechat.Entity
{
    /// <summary>
    /// 流程模板，定制流程图
    /// </summary>
    public class tbWeOAFlowNode : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 流程节点
        /// </summary>
        [Required]
        [Display(Name = "流程节点")]
        public string FlowApprovalNode { get; set; }

        /// <summary>
        /// 流程顺序
        /// </summary>
        [Required]
        [Display(Name = "流程顺序")]
        public int FlowStep { get; set; }

        /// <summary>
        /// 条件公式，当条件等于下面的字符串则执行该流程节点，动态流程顺序，如果为空则忽略
        /// </summary>
        [Display(Name = "条件公式")]
        public string ConditionalFormula { get; set; }

        /// <summary>
        /// 流程类型
        /// </summary>
        [Required]
        [Display(Name = "流程类型")]
        public virtual tbWeOAFlowType tbSysOAFlowType { get; set; }

        /// <summary>
        /// 是否关闭
        /// </summary>
        [Required]
        [Display(Name = "是否关闭")]
        public bool FlowComplete { get; set; }
    }
}
