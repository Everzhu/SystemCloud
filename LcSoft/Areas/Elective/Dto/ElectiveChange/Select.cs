using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Elective.Dto.ElectiveChange
{
    public class Select
    {

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
        /// 分组Id
        /// </summary>
        public int ElectiveGroupId { get; set; }

        /// <summary>
        /// 分段Id
        /// </summary>
        public int ElectiveSectionId { get; set; }


        [Display(Name = "选课分段")]
        public string ElectiveSectionName { get; set; }

        [Display(Name = "选课分组")]
        public string ElectiveGroupName { get; set; }

    }
}