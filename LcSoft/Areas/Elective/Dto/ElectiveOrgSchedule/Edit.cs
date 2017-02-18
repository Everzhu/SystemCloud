using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace XkSystem.Areas.Elective.Dto.ElectiveOrgSchedule
{
    public class Edit
    {
        public int Id { get; set; }

        /// <summary>
        /// 所属选课开班
        /// </summary>
        [Display(Name = "所属选课开班"), Required]
        public int ElectiveOrgId { get; set; }

        /// <summary>
        /// 星期
        /// </summary>
        [Display(Name = "星期"), Required]
        public int WeekId { get; set; }

        /// <summary>
        /// 节次
        /// </summary>
        [Display(Name = "节次"), Required]
        public int PeriodId { get; set; }
    }
}
