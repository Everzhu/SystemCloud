using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Moral.Entity
{
    /// <summary>
    /// 流动红旗
    /// </summary>
    public class tbMoralRedFlag: Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 班级
        /// </summary>
        [Display(Name ="班级"),Required]
        public Basis.Entity.tbClass tbClass { get; set; }

        /// <summary>
        /// 所属德育
        /// </summary>
        [Display(Name ="德育"),Required]
        public tbMoral tbMoral { get; set; }

        /// <summary>
        /// 本年度第几周
        /// </summary>
        [Display(Name ="周数"),Required]
        public int WeekNum { get; set; }
        
        /// <summary>
        /// 本周第一天
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// 本周最后一天
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// 录入日期
        /// </summary>
        [Display(Name ="录入日期"),Required]
        public DateTime InputDate { get; set; }

        /// <summary>
        /// 是否禁用
        /// </summary>
        [Display(Name ="是否禁用"),Required]
        public bool IsDisabled { get;  set; }
    }
}