using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Dorm.Dto.DormStudent
{
    public class Edit
    {
        public int Id { get; set; }

        /// <summary>
        /// 住宿
        /// </summary>
        [Display(Name = "住宿"), Required]
        public int DormId { get; set; }

        /// <summary>
        /// 教学楼
        /// </summary>
        [Display(Name = "教学楼"), Required]
        public int BuildId { get; set; }

        /// <summary>
        /// 宿舍名称
        /// </summary>
        [Display(Name = "宿舍名称"), Required]
        public int RoomId { get; set; }

        /// <summary>
        /// 学生学号
        /// </summary>
        [Display(Name = "学生学号"), Required]
        public string StudentCode { get; set; }
    }
}