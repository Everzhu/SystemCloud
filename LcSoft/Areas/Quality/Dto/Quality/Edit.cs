using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace XkSystem.Areas.Quality.Dto.Quality
{
    public class Edit
    {
        public int Id { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        [Display(Name = "排序")]
        public int? No { get; set; }

        /// <summary>
        /// 评教名称
        /// </summary>
        [Display(Name = "评价名称"), Required]
        public string QualityName { get; set; }

        /// <summary>
        /// 学段
        /// </summary>
        [Display(Name = "学段"), Required]
        public int YearId { get; set; }

        /// <summary>
        /// 显示状态
        /// </summary>
        [Display(Name = "显示状态"), Required]
        public bool IsOpen { get; set; } = true;

        /// <summary>
        /// 是否激活
        /// </summary>
        [Display(Name = "是否激活"), Required]
        public bool IsActive { get; set; } = false;

        /// <summary>
        /// 开始时间
        /// </summary>
        [Display(Name = "开始时间"), Required]
        public DateTime FromDate { get; set; } = DateTime.Now;

        /// <summary>
        /// 结束时间
        /// </summary>
        [Display(Name = "结束时间"), Required]
        public DateTime ToDate { get; set; } = DateTime.Now;
    }
}
