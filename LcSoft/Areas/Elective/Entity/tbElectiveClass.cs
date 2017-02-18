using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XkSystem.Areas.Elective.Entity
{
    public class tbElectiveClass : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 参选班级 
        /// </summary>
        [Required]
        [Display(Name = "参选班级")]
        public virtual Basis.Entity.tbClass tbClass { get; set; }

        /// <summary>
        /// 所属选课
        /// </summary>
        [Required]
        [Display(Name = "所属选课")]
        public virtual tbElective tbElective { get; set; }
    }
}
