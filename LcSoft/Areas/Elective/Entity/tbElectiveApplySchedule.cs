using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Elective.Entity
{
    /// <summary>
    /// 选课申报课程课表
    /// </summary>
    public class tbElectiveApplySchedule: Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 所属选课申请
        /// </summary>
        [Required]
        [Display(Name = "所属选课申请")]
        public virtual tbElectiveApply tbElectiveApply { get; set; }

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

        public int tbOpen_Id { get; set; }

    }
}