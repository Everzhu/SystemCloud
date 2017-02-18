using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Dorm.Dto.DormTeacher
{
    public class Edit
    {
        public int Id { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        [Display(Name = "排序")]
        public int? No { get; set; }

        /// <summary>
        /// 教师职工号
        /// </summary>
        [Display(Name = "教师职工号"), Required]
        public string TeacherCode { get; set; }

        /// <summary>
        /// 宿舍名称
        /// </summary>
        [Display(Name = "宿舍名称"), Required]
        public int RoomId { get; set; }

        /// <summary>
        /// 教学楼
        /// </summary>
        [Display(Name = "教学楼"), Required]
        public int BuildId { get; set; }

        /// <summary>
        /// 宿舍名称
        /// </summary>
        [Display(Name = "宿舍名称")]
        public string RoomName { get; set; }

        /// <summary>
        /// 教师姓名
        /// </summary>
        [Display(Name = "教师姓名")]
        public string TeacherName { get; set; }

        /// <summary>
        /// 宿舍楼
        /// </summary>
        [Display(Name = "宿舍楼")]
        public string BuildName { get; set; }
    }
}