using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XkSystem.Areas.Wechat.Entity
{
    /// <summary>
    /// 请假审批对象
    /// </summary>
    public class tbWeOAApplyLeave : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 用车部门
        /// </summary>
        [Required]
        [Display(Name = "用车部门")]
        public Teacher.Entity.tbTeacherDept tbTeacherDept { get; set; }

        /// <summary>
        /// 请假类别
        /// </summary>
        [Required]
        [Display(Name = "请假类别")]
        public tbWeOALeaveType tbWeOALeaveType { get; set; }

        /// <summary>
        /// 请假事由
        /// </summary>
        [Required]
        [Display(Name = "请假事由")]
        public string Reason { get; set; }

        /// <summary>
        /// 请假开始时间
        /// </summary>
        [Required]
        [Display(Name = "请假开始时间")]
        public DateTime LeaveFromTime { get; set; }

        /// <summary>
        /// 请假结束时间
        /// </summary>
        [Required]
        [Display(Name = "请假结束时间")]
        public DateTime LeaveToTime { get; set; }

        /// <summary>
        /// 请假天数,0.5,1,1.5,2,2.5,3....
        /// </summary>
        [Required]
        [Display(Name = "请假天数")]
        public float LeaveDayCount { get; set; }

        /// <summary>
        /// 病例文件
        /// </summary>
        [Display(Name = "病例文件")]
        public string CaseFileName { get; set; }
    }
}
