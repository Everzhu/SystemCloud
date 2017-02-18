﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Survey.Entity
{
    /// <summary>
    /// 评教科目
    /// </summary>
    public class tbSurveySubject : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 所属评教
        /// </summary>
        [Display(Name = "所属评教"), Required]
        public virtual tbSurveyGroup tbSurveyGroup { get; set; }

        /// <summary>
        /// 对应科目
        /// </summary>
        [Display(Name = "对应科目"), Required]
        public virtual Course.Entity.tbSubject tbSubject { get; set; }
    }
}