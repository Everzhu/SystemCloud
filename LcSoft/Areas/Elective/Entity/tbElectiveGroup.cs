using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XkSystem.Areas.Elective.Entity
{
    public class tbElectiveGroup : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 选课组名 
        /// </summary>
        [Required]
        [Display(Name = "选课组名")]
        public string ElectiveGroupName { get; set; }

        /// <summary>
        /// 所属选课
        /// </summary>
        [Required]
        [Display(Name = "所属选课")]
        public virtual tbElective tbElective { get; set; }

        /// <summary>
        /// 最大选课数
        /// </summary>
        [Required]
        [Display(Name = "最大选课数")]
        public int MaxElective { get; set; }

        /// <summary>
        /// 最小选课数
        /// </summary>
        [Required]
        [Display(Name = "最小选课数")]
        public int MinElective { get; set; }
    }
}
