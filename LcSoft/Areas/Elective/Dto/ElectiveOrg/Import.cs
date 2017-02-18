using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Elective.Dto.ElectiveOrg
{
    public class Import
    {
        /// <summary>
        /// 所属课程
        /// </summary>
        [Display(Name = "课程名称")]
        public string CourseName { get; set; }

        /// <summary>
        /// 班级名称
        /// </summary>
        [Display(Name = "班级名称")]
        public string OrgName { get; set; }

        /// <summary>
        /// 总名额
        /// </summary>
        [Display(Name = "总人数")]
        public string MaxCount { get; set; }

        /// <summary>
        /// 授权状态
        /// </summary>
        [Display(Name = "授权状态")]
        public string Permit { get; set; }

        /// <summary>
        /// 分组名
        /// </summary>
        [Display(Name = "选课分组名称")]
        public string ElectiveGroupName { get; set; }

        /// <summary>
        /// 分段名
        /// </summary>
        [Display(Name = "选课分段名称")]
        public string ElectiveSectionName { get; set; }

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
        /// 星期节次
        /// </summary>
        [Display(Name = "星期节次")]
        public string OrgSchedule { get; set; }

        /// <summary>
        /// 选课对象
        /// </summary>
        [Display(Name = "选课对象")]
        public string LimitClass { get; set; }

        /// <summary>
        /// 导入结果
        /// </summary>
        [Display(Name = "导入结果")]
        public string ImportError { get; set; }

        /// <summary>
        /// 系统存在相同班级
        /// </summary>
        public bool IsExists { get; set; }
    }
}