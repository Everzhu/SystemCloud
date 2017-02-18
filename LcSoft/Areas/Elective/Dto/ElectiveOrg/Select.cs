using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Elective.Dto.ElectiveOrg
{
    public class Select
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 班级名称
        /// </summary>
        [Display(Name = "班级名称")]
        public string OrgName { get; set; }

        /// <summary>
        /// 是否选中
        /// </summary>
        public bool IsChecked { get; set; }

        public bool IsFixed { get; set; }

        /// <summary>
        /// 教室
        /// </summary>
        [Display(Name = "教室")]
        public string RoomName { get; set; }

        /// <summary>
        /// 任课教师
        /// </summary>
        [Display(Name = "任课教师")]
        public string TeacherName { get; set; }

        /// <summary>
        /// 星期节次
        /// </summary>
        [Display(Name = "星期节次")]
        public string WeekPeriod { get; set; }

        /// <summary>
        /// 总名额
        /// </summary>
        [Display(Name = "总名额")]
        public int MaxCount { get; set; }

        /// <summary>
        /// 剩余名额
        /// </summary
        [Display(Name = "剩余名额")]
        public int RemainCount { get; set; }

        /// <summary>
        /// 授权状态
        /// </summary>
        [Display(Name = "授权状态")]
        public int Permit { get; set; }

        /// <summary>
        /// 分组Id
        /// </summary>
        public int ElectiveGroupId { get; set; }

        /// <summary>
        /// 选课分组
        /// </summary>
        [Display(Name = "选课分组")]
        public string ElectiveGroupName { get; set; }

        /// <summary>
        /// 分组最大可选人数
        /// </summary>
        public int ElectiveGroupMaxElective { get; set; }

        /// <summary>
        /// 分段Id
        /// </summary>
        public int ElectiveSectionId { get; set; }


        [Display(Name = "选课分段")]
        public string ElectiveSectionName { get; set; }

        public int WeekId { get; set; }
        public int PeriodId { get; set; }
        
        /// <summary>
        /// 课程说明
        /// </summary>
        [Display(Name = "课程说明")]
        public int CourseId { get; set; }
    }
}