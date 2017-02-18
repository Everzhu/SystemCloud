using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Basis.Entity
{
    /// <summary>
    /// 教室
    /// </summary>
    public class tbRoom : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 教室
        /// </summary>
        [Required]
        [Display(Name = "教室")]
        public string RoomName { get; set; }

        /// <summary>
        /// 教学楼
        /// </summary>
        //[Required]
        [Display(Name = "教学楼")]
        public virtual tbBuild tbBuild { get; set; }

        /// <summary>
        /// 教室类型
        /// </summary>
        [Required]
        [Display(Name = "教室类型")]
        public virtual tbRoomType tbRoomType { get; set; }

        /// <summary>
        /// 容纳人数
        /// </summary>
        [Required]
        [Display(Name = "容纳人数")]
        public int MaxUser { get; set; }

        /// <summary>
        /// 考勤机
        /// </summary>
        [Display(Name = "考勤机")]
        public string AttendanceMachine { get; set; }
    }
}