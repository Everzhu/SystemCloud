using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Survey.Entity
{
    /// <summary>
    /// 教师评价
    /// </summary>
    public class tbSurvey : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 评价名称
        /// </summary>
        [Display(Name = "评价名称"), Required]
        public string SurveyName { get; set; }

        /// <summary>
        /// 学年
        /// </summary>
        [Display(Name = "学年"), Required]
        public virtual Basis.Entity.tbYear tbYear { get; set; }

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
        /// 备注说明
        /// </summary>
        [Display(Name = "备注说明")]
        public string Remark { get; set; }
    }
}