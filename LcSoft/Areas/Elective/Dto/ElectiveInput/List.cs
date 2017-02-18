using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Elective.Dto.ElectiveInput
{
    public class List
    {
        public int Id { get; set; }

        public int? No { get; set; }

        /// <summary>
        /// 选课名称 
        /// </summary>
        [Display(Name = "选课名称"), Required]
        public string ElectiveName { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        [Display(Name = "开始时间")]
        public DateTime FromDate { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        [Display(Name = "结束时间")]
        public DateTime ToDate { get; set; }

        /// <summary>
        /// 选课模式
        /// </summary>
        [Display(Name = "选课模式")]
        public string ElectiveTypeName { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        [Display(Name = "状态")]
        public bool IsDisable { get; set; }

        /// <summary>
        /// 选课说明
        /// </summary>
        [Display(Name = "选课说明")]
        public string Remark { get; set; }

        /// <summary>
        /// 是否星期模式
        /// </summary>
        public bool IsWeekPeriod { get; set; }

        /// <summary>
        /// 是否弹窗模式
        /// </summary>
        public bool IsPop { get; set; }
    }
}