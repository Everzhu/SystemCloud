using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Wechat.Models.ApplyLeave
{
    public class ApplyLeaveListModel
    {
        public IEnumerable<Dto.ApplyLeave.ApplyLeaveListDto> MyApplyDto { get; set; } = new List<Dto.ApplyLeave.ApplyLeaveListDto>();

        public List<Dto.ApplyLeave.ApplyLeaveListDto> WaitApproveDto { get; set; } = new List<Dto.ApplyLeave.ApplyLeaveListDto>();

        public List<Dto.ApplyLeave.ApplyLeaveListDto> ApprovedDto { get; set; } = new List<Dto.ApplyLeave.ApplyLeaveListDto>();

        public List<Dto.ApplyLeave.ApplyLeaveListDto> WorkFlowListDto { get; set; } = new List<Dto.ApplyLeave.ApplyLeaveListDto>();

        public string UserListJson { get; set; }

        public string SegmentedTab { get; set; } = System.Web.HttpContext.Current.Request["SegmentedTab"].ConvertToString();
        public int ApproveStatus { get; set; } = System.Web.HttpContext.Current.Request["ApproveStatus"].ConvertToInt();
        

        public Code.PageHelper myPage { get; set; } = new Code.PageHelper();
        public Code.PageHelper waitPage { get; set; } = new Code.PageHelper();
        public Code.PageHelper approvedPage { get; set; } = new Code.PageHelper();

        public bool StartFlowPermission { get; set; }

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();

    }
}