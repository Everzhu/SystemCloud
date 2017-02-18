using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace XkSystem.Areas.Course.Dto.Org
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
        /// 教学班
        /// </summary>
        [Display(Name = "教学班"), Required]
        public string OrgName { get; set; }

        /// <summary>
        /// 学年
        /// </summary>
        [Display(Name = "学年"), Required]
        public int? YearId { get; set; }

        /// <summary>
        /// 课程
        /// </summary>
        [Display(Name = "课程"), Required]
        public int CourseId { get; set; }

        /// <summary>
        /// 年级
        /// </summary>
        [Display(Name = "年级"), Required]
        public int GradeId { get; set; }

        /// <summary>
        /// 任课教师
        /// </summary>
        [Display(Name = "任课教师")]
        public string TeacherId { get; set; }

        /// <summary>
        /// 上课教室
        /// </summary>
        [Display(Name = "上课教室")]
        public int? RoomId { get; set; }

        /// <summary>
        /// 行政班
        /// </summary>
        [Display(Name = "行政班")]
        public int? ClassId { get; set; }

        /// <summary>
        /// 班级模式
        /// </summary>
        [Display(Name = "班级模式"), Required]
        public bool IsClass { get; set; }

        /// <summary>
        /// 自动考勤
        /// </summary>
        [Display(Name = "自动考勤"), Required]
        public bool IsAutoAttendance { get; set; }
    }
}
