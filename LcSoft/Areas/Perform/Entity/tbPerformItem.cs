using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Perform.Entity
{
    /// <summary>
    /// 学生评价项目
    /// </summary>
    public class tbPerformItem : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 评价项目
        /// </summary>
        [Required]
        [Display(Name = "评价项目")]
        public string PerformItemName { get; set; }
        /// <summary>
        /// 评价分组
        /// </summary>
        [Required]
        [Display(Name = "评价分组")]
        public virtual tbPerformGroup tbPerformGroup { get; set; }
        /// <summary>
        /// 满分值
        /// </summary>
        [Required]
        [Display(Name = "满分值")]
        public string ScoreMax { get; set; }
        /// <summary>
        /// 折算比例
        /// </summary>
        [Required]
        [Display(Name = "折算比例")]
        public decimal Rate { get; set; }
        /// <summary>
        /// 是否选项
        /// </summary>
        [Required]
        [Display(Name = "是否选项")]
        public bool IsSelect { get; set; }
        /// <summary>
        /// 是否多次
        /// </summary>
        [Required]
        [Display(Name = "是否多次")]
        public bool IsMany { get; set; }
        /// <summary>
        /// 默认分数
        /// </summary>
        [Required]
        [Display(Name = "默认分数")]
        public decimal DefaultValue { get; set; }
    }
}