using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Course.Dto.Org
{
    public class ImportOrg
    {
        /// <summary>
        /// 班序
        /// </summary>
        [Display(Name = "班序")]
        public string No { get; set; }

        /// <summary>
        /// 教学班名称
        /// </summary>
        [Display(Name = "教学班名称")]
        public string OrgName { get; set; }

        /// <summary>
        /// 学年
        /// </summary>
        [Display(Name = "学年")]
        public string YearName { get; set; }

        /// <summary>
        /// 年级
        /// </summary>
        [Display(Name = "年级")]
        public string GradeName { get; set; }

        /// <summary>
        /// 课程
        /// </summary>
        [Display(Name = "课程")]
        public string CourseName { get; set; }

        /// <summary>
        /// 班级模式
        /// </summary>
        [Display(Name = "班级模式")]
        public string IsClass { get; set; }

        [Display(Name = "绑定班级")]
        public string ClassName { get; set; }

        /// <summary>
        /// 任课老师姓名
        /// </summary>
        [Display(Name = "任课老师")]
        public List<string> TeacherName { get; set; }

        /// <summary>
        /// 上课教室
        /// </summary>
        [Display(Name = "上课教室")]
        public string RoomName { get; set; }

        /// <summary>
        /// 课表节次
        /// </summary>
        [Display(Name = "课表节次")]
        public string OrgSchedule { get; set; }

        /// <summary>
        /// 自动考勤
        /// </summary>
        [Display(Name = "自动考勤")]
        public string IsAutoAttendance { get; set; }

        /// <summary>
        /// 导入提示
        /// </summary>
        [Display(Name = "导入提示")]
        public string Error { get; set; }
    }
}