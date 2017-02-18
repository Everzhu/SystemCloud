using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Quality.Entity
{
    public class tbQuality : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 评价名称
        /// </summary>
        [Display(Name = "评价名称"), Required]
        public string QualityName { get; set; }

        /// <summary>
        /// 学段
        /// </summary>
        [Display(Name = "学段"), Required]
        public Basis.Entity.tbYear tbYear { get; set; }

        /// <summary>
        /// 评价开始时间
        /// </summary>
        [Display(Name = "评价开始时间"), Required]
        public DateTime FromDate { get; set; }

        /// <summary>
        /// 评价结束时间
        /// </summary>
        [Display(Name = "评价结束时间"), Required]
        public DateTime ToDate { get; set; }

        /// <summary>
        /// 是否开放
        /// </summary>
        [Display(Name = "是否开放"), Required]
        public bool IsOpen { get; set; }

        /// <summary>
        /// 是否激活
        /// </summary>
        [Display(Name = "是否激活"), Required]
        public bool IsActive { get; set; }
    }
}