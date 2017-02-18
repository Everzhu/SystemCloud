using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Basis.Dto.Room
{
    public class Edit
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        [Display(Name = "排序")]
        public int? No { get; set; }

        /// <summary>
        /// 教室
        /// </summary>
        [Display(Name = "教室"), Required]
        public string RoomName { get; set; }

        /// <summary>
        /// 教学楼
        /// </summary>
        [Display(Name = "教学楼"), Required]
        public int BuildId { get; set; }

        /// <summary>
        /// 教室类型
        /// </summary>
        [Display(Name = "教室类型"), Required]
        public int RoomTypeId { get; set; }

        /// <summary>
        /// 容纳人数
        /// </summary>
        [Display(Name = "容纳人数"), Required]
        [Range(0, 10000)]
        public int MaxUser { get; set; }

        /// <summary>
        /// 考勤机
        /// </summary>
        [Display(Name = "考勤机")]
        public string AttendanceMachine { get; set; }
    }
}