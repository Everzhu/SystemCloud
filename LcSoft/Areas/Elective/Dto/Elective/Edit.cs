using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace XkSystem.Areas.Elective.Dto.Elective
{
    public class Edit
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
        [Display(Name = "选课名称"), Required]
        public string ElectiveName { get; set; }

        /// <summary>
        /// 选课开始时间
        /// </summary>
        [Display(Name = "选课时间"), Required]
        public DateTime FromDate { get; set; } = DateTime.Now;

        /// <summary>
        /// 选课结束时间
        /// </summary>
        [Display(Name = "结束时间"), Required]
        public DateTime ToDate { get; set; } = DateTime.Now;

        /// <summary>
        /// 申报开始时间
        /// </summary>
        [Display(Name = "申报时间"), Required]
        public DateTime ApplyFromDate { get; set; } = DateTime.Now;

        /// <summary>
        /// 申报结束时间
        /// </summary>
        [Display(Name = "申报结束时间"), Required]
        public DateTime ApplyToDate { get; set; } = DateTime.Now;

        /// <summary>
        /// 选课模式
        /// </summary>
        [Display(Name = "选课模式"), Required]
        public int ElectiveTypeId { get; set; }

        /// <summary>
        /// 弹窗模式
        /// </summary>
        [Display(Name = "弹窗模式"), Required]
        public bool IsPop { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        [Display(Name = "状态"), Required]
        public bool IsDisable { get; set; }

        /// <summary>
        /// 选课说明
        /// </summary>
        [Display(Name = "选课说明")]
        public string Remark { get; set; }
    }
}
