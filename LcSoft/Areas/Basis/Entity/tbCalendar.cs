using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XkSystem.Areas.Basis.Entity
{
    /// <summary>
    /// 校历设置
    /// </summary>
    public class tbCalendar  : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 学年
        /// </summary>
        [Required]
        [Display(Name = "学年")]
        public virtual tbYear tbYear { get; set; }

        /// <summary>
        /// 校历
        /// </summary>
        [Required]
        [Display(Name = "校历")]
        public DateTime CalendarDate { get; set; }

        /// <summary>
        /// 星期
        /// </summary>
        [Display(Name = "星期")]
        public virtual tbWeek tbWeek { get; set; }

        /// <summary>
        /// 备注信息
        /// </summary>
        [Display(Name = "备注信息")]
        public string Remark { get; set; }
    }
}
