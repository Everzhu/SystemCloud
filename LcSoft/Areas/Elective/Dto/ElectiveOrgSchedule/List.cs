using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace XkSystem.Areas.Elective.Dto.ElectiveOrgSchedule
{
    public class List
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 选课开班Id
        /// </summary>
        [Display(Name = "所属选课开班Id")]
        public int ElectiveOrgId { get; set; }

        /// <summary>
        /// 所属选课开班
        /// </summary>
        [Display(Name = "所属选课开班")]
        public string ElectiveOrgName { get; set; }

        /// <summary>
        /// 星期Id
        /// </summary>
        [Display(Name = "星期Id")]
        public int WeekId { get; set; }

        /// <summary>
        /// 星期
        /// </summary>
        [Display(Name = "星期")]
        public string WeekName { get; set; }

        /// <summary>
        /// 节次Id
        /// </summary>
        [Display(Name = "节次Id")]
        public int PeriodId { get; set; }

        /// <summary>
        /// 节次
        /// </summary>
        [Display(Name = "节次")]
        public string PeriodName { get; set; }
    }
}
