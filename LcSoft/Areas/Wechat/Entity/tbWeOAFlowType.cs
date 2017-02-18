using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XkSystem.Areas.Wechat.Entity
{
    /// <summary>
    /// 流程分类,审批项目
    /// </summary>
    public class tbWeOAFlowType : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 流程类型标识符
        /// </summary>
        [Required]
        [Display(Name = "流程类型标识符")]
        public string Code { get; set; }

        /// <summary>
        /// 流程类型名称
        /// </summary>
        [Required]
        [Display(Name = "流程类型名称")]
        public string FlowTypeName { get; set; }
    }
}
