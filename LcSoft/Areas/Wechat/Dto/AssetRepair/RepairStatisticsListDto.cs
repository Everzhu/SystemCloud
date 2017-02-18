using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LcSoft.Areas.Wechat.Dto.AssetRepair
{
    public class RepairStatisticsListDto
    {
        public int Id { get; set; }

        /// <summary>
        /// 报修人员
        /// </summary>
        [Display(Name = "报修人员")]
        public string UserName { get; set; }

        /// <summary>
        /// 接受数量
        /// </summary>
        [Display(Name = "接受数量")]
        public int AcceptedCount { get; set; }

        /// <summary>
        /// 完成数量
        /// </summary>
        [Display(Name = "完成数量")]
        public int FinishCount { get; set; }

        /// <summary>
        /// 好评数量
        /// </summary>
        [Display(Name = "好评数量")]
        public int SatisfiedCount { get; set; }

        /// <summary>
        /// 差评数量
        /// </summary>
        [Display(Name = "差评数量")]
        public int BadReviewCount { get; set; }
    }
}