using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LcSoft.Areas.Wechat.Models.AssetRepair
{
    public class RepairStatisticsInfoModel
    {
        public List<Asset.Dto.AssetRepair.List> AssetRepairList { get; set; } = new List<Asset.Dto.AssetRepair.List>();

        public Code.PageHelper Page { get; set; } = new Code.PageHelper();

        /// <summary>
        /// 0:受理数量；1：完成数量；2：好评数量；3：差评数量
        /// </summary>
        public int? InfoType { get; set; } = System.Web.HttpContext.Current.Request["InfoType"].ConvertToInt();//Wechat

        public int? UserId { get; set; } = System.Web.HttpContext.Current.Request["UserId"].ConvertToInt();//Wechat
    }
}