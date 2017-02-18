using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace XkSystem.Areas.Course.Dto.Org
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
        /// 教学班
        /// </summary>
        [Display(Name = "教学班")]
        public string OrgName { get; set; }

        /// <summary>
        /// 教学模式
        /// </summary>
        [Display(Name = "教学模式")]
        public bool IsClass { get; set; }

        /// <summary>
        /// 绑定行政班
        /// </summary>
        [Display(Name = "绑定行政班")]
        public string ClassName { get; set; }

        /// <summary>
        /// 学年学段
        /// </summary>
        [Display(Name = "学年学段")]
        public string YearName { get; set; }

        /// <summary>
        /// 科目
        /// </summary>
        [Display(Name = "科目")]
        public string SubjectName { get; set; }

        /// <summary>
        /// 课程
        /// </summary>
        [Display(Name = "课程")]
        public string CourseName { get; set; }

        /// <summary>
        /// 年级
        /// </summary>
        [Display(Name = "年级")]
        public string GradeName { get; set; }

        /// <summary>
        /// 年级
        /// </summary>
        [Display(Name = "班级")]
        public string OrgTypeName { get; set; }

        /// <summary>
        /// 任课教师
        /// </summary>
        [Display(Name = "任课教师")]
        public string TeacherName { get; set; }

        /// <summary>
        /// 教室
        /// </summary>
        [Display(Name = "教室")]
        public string RoomName { get; set; }

        /// <summary>
        /// Total
        /// </summary>
        [Display(Name = "人数")]
        public int StudentCount { get; set; }

        /// <summary>
        /// 课表节次
        /// </summary>
        [Display(Name = "课表节次")]
        public string Schedule { get; set; }

        public string ScheduleString { get; set; }

        /// <summary>
        /// 自动考勤
        /// </summary>
        [Display(Name = "自动考勤")]
        public string IsAutoAttendance { get; set; }
    }
}
