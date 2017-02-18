using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XkSystem.Areas.Wechat.Entity
{
    /// <summary>
    /// 用车申请审批对象
    /// </summary>
    public class tbWeOAApplyCar : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 用车部门
        /// </summary>
        [Required]
        [Display(Name = "用车部门")]
        public virtual Teacher.Entity.tbTeacherDept tbTeacherDept { get; set; }

        /// <summary>
        /// 目的地
        /// </summary>
        [Required]
        [Display(Name = "目的地")]
        public string Destination { get; set; }

        /// <summary>
        /// 用车原因
        /// </summary>
        [Display(Name = "用车原因")]
        public string Reason { get; set; }

        /// <summary>
        /// 用车时间
        /// </summary>
        [Display(Name = "用车时间")]
        [Required]
        public DateTime CarTime { get; set; }

        /// <summary>
        /// 随行人员
        /// </summary>
        [Display(Name = "随行人员")]
        public string  OtherUsers { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [Display(Name = "备注")]
        public string Remark { get; set; }
    }
}
