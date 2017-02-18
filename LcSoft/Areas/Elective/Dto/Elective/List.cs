using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace XkSystem.Areas.Elective.Dto.Elective
{
    public class List
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        [Display(Name = "排序")]
        public int? No { get; set; }

        /// <summary>
        /// 选课名称 
        /// </summary>
        [Display(Name = "选课名称")]
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
        /// 弹窗模式
        /// </summary>
        [Display(Name = "弹窗模式")]
        public bool IsPop { get; set; }

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
    }
}
