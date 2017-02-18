using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Course.Dto.Schedule
{
    public class ImportClassSchedule
    {
        [Display(Name ="班序")]
        public int? No { get; set; }

        [Display(Name = "班级名称")]
        public string ClassName { get; set; }

        [Display(Name = "年级")]
        public string GradeName { get; set; }

        [Display(Name = "课程")]
        public string CourseName { get; set; }

        [Display(Name = "星期")]
        public string WeekName { get; set; }

        [Display(Name = "节次")]
        public string PeriodName { get; set; }

        [Display(Name = "任课老师")]
        public string TeacherName { get; set; }

        [Display(Name = "任课老师编号")]
        public string TeacherCode { get; set; }

        [Display(Name = "教室")]
        public string RoomName { get; set; }

        [Display(Name = "错误信息")]
        public string Error { get; set; }
    }
}