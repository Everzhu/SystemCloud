using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XkSystem.Areas.Elective.Entity
{
    /// <summary>
    /// 选课课表
    /// </summary>
    public class tbElectiveOrgSchedule : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 所属选课开班
        /// </summary>
        [Required]
        [Display(Name = "所属选课开班")]
        public virtual tbElectiveOrg tbElectiveOrg { get; set; }

        /// <summary>
        /// 星期
        /// </summary>
        [Required]
        [Display(Name = "星期")]
        public virtual Basis.Entity.tbWeek tbWeek { get; set; }

        /// <summary>
        /// 节次
        /// </summary>
        [Required]
        [Display(Name = "节次")]
        public virtual Basis.Entity.tbPeriod tbPeriod { get; set; }
    }
}