using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace XkSystem.Areas.Perform.Dto.PerformItem
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
        /// 评价项目
        /// </summary>
        [Display(Name = "评价项目")]
        public string PerformItemName { get; set; }
        /// <summary>
        /// 评价分组
        /// </summary>
        [Display(Name = "评价分组")]
        public int PerformGroupId { get; set; }
        /// <summary>
        /// 评价分组
        /// </summary>
        [Display(Name = "评价分组")]
        public string PerformGroupName { get; set; }
        /// <summary>
        /// 满分值
        /// </summary>
        [Display(Name = "满分值")]
        public string ScoreMax { get; set; }
        /// <summary>
        /// 折算比例
        /// </summary>
        [Display(Name = "折算比例%")]
        public decimal Rate { get; set; }
        /// <summary>
        /// 是否选项
        /// </summary>
        [Display(Name = "是否选项")]
        public bool IsSelect { get; set; }
        /// <summary>
        /// 是否多次
        /// </summary>
        [Display(Name = "是否多次")]
        public bool IsMany { get; set; }
        /// <summary>
        /// 默认分数
        /// </summary>
        [Display(Name = "默认分数")]
        public decimal DefaultValue { get; set; }
    }
}
