using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LcSoft.Areas.Wechat.Dto.AssetRepair
{
    public class AssetFeedBackEditDto
    {
        /// <summary>
        /// 反馈内容
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 图片名称
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// 所属报修
        /// </summary>
        public int tbAssetRepairId { get; set; }
    }
}