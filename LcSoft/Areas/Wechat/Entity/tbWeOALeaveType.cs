using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XkSystem.Areas.Wechat.Entity
{
    /// <summary>
    /// 请假类型
    /// </summary>
    public class tbWeOALeaveType : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 列别名称
        /// </summary>
        [Required]
        [Display(Name = "列别名称")]
        public string LeaveTypeName { get; set; }
    }
}
