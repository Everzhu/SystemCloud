using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Perform.Dto.PerformData
{
    public class OrgSelectInfo
    {
        /// <summary>
        /// 排序
        /// </summary>
        [Display(Name = "排序")]
        public int? No { get; set; }

        /// <summary>
        /// 教学班Id
        /// </summary>
        [Display(Name = "教学班Id")]
        public int OrgId { get; set; }

        /// <summary>
        /// 教学班
        /// </summary>
        [Display(Name = "教学班")]
        public string OrgName { get; set; }

        /// <summary>
        /// 已评人数
        /// </summary>
        [Display(Name = "已评人数")]
        public int Count { get; set; }

        /// <summary>
        /// 全部人数
        /// </summary>
        [Display(Name = "全部人数")]
        public int SumCount { get; set; }
    }
}