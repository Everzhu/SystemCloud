using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Elective.Entity
{
    /// <summary>
    /// 选课科目
    /// </summary>
    public class tbElectiveSubject: Code.EntityHelper.EntityBase
    {
        [Display(Name ="所属选课"),Required]
        public virtual tbElective tbElective { get; set; }

        [Display(Name ="科目"),Required]
        public virtual Course.Entity.tbSubject tbSubject { get; set; }
    }
}