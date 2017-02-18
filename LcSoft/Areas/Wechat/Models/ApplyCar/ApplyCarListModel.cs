using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Wechat.Models.ApplyCar
{
    public class ApplyCarListModel
    {
        public IEnumerable<Dto.ApplyCar.ApplyCarListDto> MyApplyDto { get; set; } = new List<Dto.ApplyCar.ApplyCarListDto>();

        public List<Dto.ApplyCar.ApplyCarListDto> WaitApproveDto { get; set; } = new List<Dto.ApplyCar.ApplyCarListDto>();

        public List<Dto.ApplyCar.ApplyCarListDto> ApprovedDto { get; set; } = new List<Dto.ApplyCar.ApplyCarListDto>();

        public List<Dto.ApplyCar.ApplyCarListDto> WorkFlowListDto { get; set; } = new List<Dto.ApplyCar.ApplyCarListDto>();

        public string SegmentedTab { get; set; } = System.Web.HttpContext.Current.Request["SegmentedTab"].ConvertToString();
        public int ApproveStatus { get; set; } = System.Web.HttpContext.Current.Request["ApproveStatus"].ConvertToInt();
        

        public string UserListJson { get; set; }

        public Code.PageHelper myPage { get; set; } = new Code.PageHelper();
        public Code.PageHelper waitPage { get; set; } = new Code.PageHelper();
        public Code.PageHelper approvedPage { get; set; } = new Code.PageHelper();

        public bool StartFlowPermission { get; set; }

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();

    }
}