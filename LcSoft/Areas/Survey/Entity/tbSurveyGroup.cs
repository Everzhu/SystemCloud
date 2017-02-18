using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Survey.Entity
{
    /// <summary>
    /// 评价分组
    /// </summary>
    public class tbSurveyGroup : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 评价分组
        /// </summary>
        [Display(Name = "评价分组"), Required]
        public string SurveyGroupName { get; set; }

        /// <summary>
        /// 评价
        /// </summary>
        [Display(Name = "评价"), Required]
        public virtual tbSurvey tbSurvey { get; set; }

        /// <summary>
        /// 评教方式
        /// </summary>
        [Display(Name = "评教方式"), Required]
        public bool IsOrg { get; set; }
    }
}