using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Basis.Dto.Room
{
    public class Export
    {
        /// <summary>
        /// 排序
        /// </summary>
        [Display(Name = "排序")]
        public int? No { get; set; }

        /// <summary>
        /// 容纳人数
        /// </summary>
        [Display(Name = "容纳人数")]
        public int? MaxUser { get; set; }

        /// <summary>
        /// 教室
        /// </summary>
        [Display(Name = "教室")]
        public string RoomName { get; set; }

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
    }
}