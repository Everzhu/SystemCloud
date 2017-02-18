using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Survey.Entity
{
    /// <summary>
    /// 参评班级
    /// </summary>
    public class tbSurveyClass : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 所属评教
        /// </summary>
        [Display(Name = "所属评教"), Required]
        public virtual tbSurvey tbSurvey { get; set; }

        /// <summary>
        /// 参评班级
        /// </summary>
        [Display(Name = "参评班级"), Required]
        public virtual Basis.Entity.tbClass tbClass { get; set; }
    }
}