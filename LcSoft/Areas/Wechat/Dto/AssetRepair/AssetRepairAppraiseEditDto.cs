using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LcSoft.Areas.Wechat.Dto.AssetRepair
{
    public class AssetRepairAppraiseEditDto
    {
        /// <summary>
        /// 是否满意
        /// </summary>
        [Display(Name = "是否满意")]
        public bool IsPleased { get; set; }

        /// <summary>
        /// 服务质量
        /// </summary>
        [Display(Name = "服务质量")]
        public bool IsService { get; set; }

        /// <summary>
        /// 意见
        /// </summary>
        [Display(Name = "意见")]
        public string Opinion { get; set; }


        /// <summary>
        /// 所属报修
        /// </summary>
        public int tbAssetRepairId { get; set; }

        /// <summary>
        /// 是否查看评价
        /// </summary>
        public bool IsView { get; set; }
    }
}