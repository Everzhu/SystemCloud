using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Wechat.Models.Office
{
    public class OfficeListModel
    {
        public IEnumerable<Dto.Office.OfficeListDto> MyApplyDto { get; set; } = new List<Dto.Office.OfficeListDto>();

        public List<Dto.Office.OfficeListDto> WaitApproveDto { get; set; } = new List<Dto.Office.OfficeListDto>();

        public List<Dto.Office.OfficeListDto> ApprovedDto { get; set; } = new List<Dto.Office.OfficeListDto>();

        public List<Dto.Office.OfficeListDto> WorkFlowListDto { get; set; } = new List<Dto.Office.OfficeListDto>();

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