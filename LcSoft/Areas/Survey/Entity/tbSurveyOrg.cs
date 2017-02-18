using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Survey.Entity
{
    /// <summary>
    /// 评教班级
    /// </summary>
    public class tbSurveyOrg : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 所属评教
        /// </summary>
        [Display(Name = "所属评教"), Required]
        public virtual tbSurveyGroup tbSurveyGroup { get; set; }

        /// <summary>
        /// 评教班级
        /// </summary>
        [Display(Name = "评教班级"), Required]
        public virtual Course.Entity.tbOrg tbOrg { get; set; }
    }
}