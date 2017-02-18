using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Wechat.Dto.WeApprover
{
    public class List
    {
        public int Id { get; set; }

        ///// <summary>
        ///// 审批人名称
        ///// </summary>
        //[Display(Name = "审批人名称")]
        //public string ApproveUserName { get; set; }

        /// <summary>
        /// 流程类型
        /// </summary>
        [Display(Name = "流程类型")]
        public string FlowTypeName { get; set; }

        /// <summary>
        /// 审批节点名称
        /// </summary>
        [Display(Name = "审批节点名称")]
        public string ApproveNodeName { get; set; }
    }
}