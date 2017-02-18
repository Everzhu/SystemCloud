using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Study.Dto.StudyTimetable
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
        /// 教师
        /// </summary>
        [Display(Name = "教师姓名")]
        public string TeacherName { get; set; }
        /// <summary>
        /// 年级
        /// </summary>
        [Display(Name = "年级")]
        public string GradeName { get; set; }
        /// <summary>
        /// 考勤次数
        /// </summary>
        [Display(Name = "考勤次数")]
        public decimal ValueCount { get; set; }
        /// <summary>
        /// 津贴类别
        /// </summary>
        [Display(Name = "津贴类别")]
        public string TypeName { get; set; }

        /// <summary>
        /// 节次费用（元）
        /// </summary>
        [Display(Name = "节次费用（元）")]
        public string PeriodMoney { get; set; }
        /// <summary>
        /// 津贴
        /// </summary>
        [Display(Name = "津贴（元)")]
        public string Allowance { get; set; }

        public int TeacherId { get; set; }
        public int SubjectId { get; set; }
        public decimal PeriodCount { get; set; }
        public decimal PeriodPreCount { get; set; }
        public string SubjectName { get; set; }
        public string OrgName { get; set; }
        public decimal ClassHour { get; set; }
        public decimal TotalMoney { get; set; }
    }
}