using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace XkSystem.Areas.Perform.Dto.PerformItem
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
        /// 评价项目
        /// </summary>
        [Display(Name = "评价项目"), Required]
        public string PerformItemName { get; set; }

        /// <summary>
        /// 评价分组
        /// </summary>
        [Display(Name = "评价分组"), Required]
        public int PerformGroupId { get; set; }

        /// <summary>
        /// 满分值
        /// </summary>
        [Display(Name = "满分值"), Required]
        public string ScoreMax { get; set; }

        /// <summary>
        /// 折算比例
        /// </summary>
        [Display(Name = "折算比例"), Range(0, 100)]
        public decimal Rate { get; set; } = 100;
        /// <summary>
        /// 是否选项
        /// </summary>
        [Required]
        [Display(Name = "是否选项")]
        public bool IsSelect { get; set; } = false;
        /// <summary>
        /// 是否多次
        /// </summary>
        [Required]
        [Display(Name = "是否多次")]
        public bool IsMany { get; set; } = false;
        /// <summary>
        /// 默认分数
        /// </summary>
        [Required]
        [Display(Name = "默认分数")]
        public decimal DefaultValue { get; set; }
    }
}
