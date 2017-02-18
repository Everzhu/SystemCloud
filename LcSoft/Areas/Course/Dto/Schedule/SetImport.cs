using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.ComponentModel.DataAnnotations;

namespace XkSystem.Areas.Course.Dto.Schedule
{
    public class SetImport
    {
        /// <summary>
        /// 行政班级ID
        /// </summary>
        public int ClassId { get; set; }

        /// <summary>
        /// 行政班级名称
        /// </summary>
        [Display(Name = "行政班级名称")]
        public string ClassName { get; set; }

        /// <summary>
        /// 行政班级排序
        /// </summary>
        public int ClassNo { get; set; }

        public List<SetImportItem> SetImportItemList { get; set; } = new List<SetImportItem>();
    }

    public class SetImportItem
    {
        /// <summary>
        /// 课程ID
        /// </summary>
        public int CourseId { get; set; }

        /// <summary>
        /// 课程名称
        /// </summary>
        [Display(Name = "课程名称")]
        public string CourseName { get; set; }

        /// <summary>
        /// 教师ID
        /// </summary>
        public int TeacherId { get; set; }

        /// <summary>
        /// 教师姓名
        /// </summary>
        [Display(Name = "教师姓名")]
        public string TeacherName { get; set; }

        /// <summary>
        /// 星期ID
        /// </summary>
        public int WeekId { get; set; }

        /// <summary>
        /// 星期名称
        /// </summary>
        [Display(Name = "星期名称")]
        public string WeekName { get; set; }

        /// <summary>
        /// 节次ID
        /// </summary>
        public int PeriodId { get; set; }

        /// <summary>
        /// 节次名称
        /// </summary>
        [Display(Name = "节次名称")]
        public string PeriodName { get; set; }
    }
}