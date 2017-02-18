using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Study.Entity
{
    public class tbStudyCost : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 所属教师
        /// </summary>
        [Display(Name = "所属教师"), Required]
        public Teacher.Entity.tbTeacher tbTeacher { get; set; }

        /// <summary>
        /// 费用
        /// </summary>
        [Display(Name = "费用"), Required]
        public decimal Cost { get; set; }
    }
}