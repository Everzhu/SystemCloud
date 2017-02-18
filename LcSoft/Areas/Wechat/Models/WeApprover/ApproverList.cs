using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Wechat.Models.WeApprover
{
    public class ApproverList
    {
        public Dto.WeApprover.ApproverList WeApprover { get; set; } = new Dto.WeApprover.ApproverList();

        public List<Dto.WeApprover.ApproverList> WeApproverList { get; set; } = new List<Dto.WeApprover.ApproverList>();

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();

        public int FlowApprovalNodeId { get; set; } = System.Web.HttpContext.Current.Request["FlowApprovalNodeId"].ConvertToInt();

        public string FlowApprovalNodeName { get; set; }

        public Code.PageHelper Page { get; set; } = new Code.PageHelper();
    }
}