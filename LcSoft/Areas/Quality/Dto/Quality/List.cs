using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace XkSystem.Areas.Quality.Dto.Quality
{
    public class List
    {
        public int Id { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        [Display(Name = "排序")]
        public int? No { get; set; }

        /// <summary>
        /// 评价名称
        /// </summary>
        [Display(Name = "评价名称")]
        public string QualityName { get; set; }

        /// <summary>
        /// 学期
        /// </summary>
        [Display(Name = "学期")]
        public string YearName { get; set; }

        /// <summary>
        /// 评价开始时间
        /// </summary>
        [Display(Name = "开始时间")]
        public DateTime FromDate { get; set; }

        /// <summary>
        /// 评价结束时间
        /// </summary>
        [Display(Name = "结束时间")]
        public DateTime ToDate { get; set; }

        /// <summary>
        /// 显示状态
        /// </summary>
        [Display(Name = "显示状态")]
        public bool IsOpen { get; set; }

        /// <summary>
        /// 激活
        /// </summary>
        [Display(Name = "激活")]
        public bool IsActive { get; set; }
    }
}
