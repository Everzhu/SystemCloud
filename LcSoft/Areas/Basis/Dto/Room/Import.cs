using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Basis.Dto.Room
{
    public class Import
    {
        /// <summary>
        /// 房间号
        /// </summary>
        [Display(Name = "房间号")]
        public string No { get; set; }

        /// <summary>
        /// 教室
        /// </summary>
        [Display(Name = "教室")]
        public string RoomName { get; set; }

        /// <summary>
        /// 容纳人数
        /// </summary>
        [Display(Name = "容纳人数")]
        public string MaxUser { get; set; }

        /// <summary>
        /// 教学楼
        /// </summary>
        [Display(Name = "教学楼")]
        public string BuildName { get; set; }

        /// <summary>
        /// 教室类型
        /// </summary>
        [Display(Name = "教室类型")]
        public string RoomTypeName { get; set; }

        /// <summary>
        /// 考勤机
        /// </summary>
        [Display(Name = "考勤机")]
        public string AttendanceMachine { get; set; }

        /// <summary>
        /// 导入提示
        /// </summary>
        [Display(Name = "导入提示")]
        public string Error { get; set; }
    }
}