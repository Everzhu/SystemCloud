using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Elective.Dto.ElectiveChange
{
    public class List
    {
        public int ElectiveOrgId { get; set; }

        public int ElectiveDataId { get; set; }

        /// <summary>
        /// 班级名称
        /// </summary>
        [Display(Name = "班级名称")]
        public string OrgName { get; set; }

        public bool IsFixed { get; set; }

        /// <summary>
        /// 是否预选
        /// </summary>
        public bool IsPreElective { get; set; }

        /// <summary>
        /// 课程Id
        /// </summary>
        public int CourseId { get; set; }

        /// <summary>
        /// 课程
        /// </summary>
        [Display(Name = "课程")]
        public string CourseName { get; set; }

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

        [Display(Name ="节次")]
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
        /// 预选学生
        /// </summary>
        [Display(Name = "预选学生")]
        public int OrgStudentCount { get; set; }
        

        /// <summary>
        /// 选课分组
        /// </summary>
        [Display(Name = "选课分组")]
        public string ElectiveGroupName { get; set; }

        [Display(Name = "选课分段")]
        public string ElectiveSectionName { get; set; }
        
        public int WeekId { get; set; }

        public int PeriodId { get; set; }

        public DateTime InputDate { get; set; }

        public int StudentUserId { get; set; }

        public int StudentId { get; set; }

        public string StudentCode { get; set; }

        public string StudentName { get; set; }

        public string ClassName { get; set; }

        public string HeadTeacherName { get; set; }
    }
}