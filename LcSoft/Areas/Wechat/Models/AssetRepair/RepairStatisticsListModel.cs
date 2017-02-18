using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LcSoft.Areas.Wechat.Models.AssetRepair
{
    public class RepairStatisticsListModel
    {
        public List<Dto.AssetRepair.RepairStatisticsListDto> RepairStatisticsListDto { get; set; } = new List<Dto.AssetRepair.RepairStatisticsListDto>();

        public Code.PageHelper Page { get; set; } = new Code.PageHelper();

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();
    }
}