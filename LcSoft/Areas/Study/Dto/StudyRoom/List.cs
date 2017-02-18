using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Study.Dto.StudyRoom
{
    public class List
    {
        public int Id { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        [Display(Name = "排序")]
        public int? No { get; set; }
        /// <summary>
        /// 晚自习名称
        /// </summary>
        [Display(Name = "晚自习名称")]
        public string StudyName { get; set; }
        /// <summary>
        /// 教室
        /// </summary>
        [Display(Name = "教室")]
        public string RoomName { get; set; }
        /// <summary>
        /// 教室Id
        /// </summary>
        [Display(Name = "教室Id")]
        public int RoomId { get; set; }
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
        /// 容纳人数
        /// </summary>
        [Display(Name = "容纳人数")]
        public int MaxUser { get; set; }
        /// <summary>
        /// 学生人数
        /// </summary>
        [Display(Name = "学生人数")]
        public int StudentCount { get; set; }
        /// <summary>
        /// 教管
        /// </summary>
        [Display(Name = "教管")]
        public string TeacherName { get; set; }
    }
}