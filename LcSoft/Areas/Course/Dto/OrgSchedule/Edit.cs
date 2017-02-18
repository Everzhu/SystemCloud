using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace XkSystem.Areas.Course.Dto.OrgSchedule
{
    public class Edit
    {
        public int Id { get; set; }

        [Display(Name = "教学班"), Required]
        public int OrgId { get; set; }

        [Display(Name = "星期"), Required]
        public int WeekId { get; set; }

        /// <summary>
        /// 节次
        /// </summary>
        [Display(Name = "节次"), Required]
        public int PeriodId { get; set; }
    }
}
