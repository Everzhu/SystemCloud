using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace XkSystem.Areas.Elective.Dto.ElectiveOrg
{
    public class List
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }

        public int? No { get; set; }

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
        /// IsFixed
        /// </summary>
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
        /// 已选人数
        /// </summary>
        [Display(Name = "已选人数")]
        public int ElectiveCount { get; set; }

        /// <summary>
        /// 授权状态
        /// </summary>
        [Display(Name = "授权状态")]
        public int Permit { get; set; }

        /// <summary>
        /// 限制班级
        /// </summary>
        [Display(Name = "限制班级")]
        public bool IsPermitClass { get; set; }

        /// <summary>
        /// 选课对象数量
        /// </summary>
        public int LimitClassCount { get; set; }

        /// <summary>
        /// 选课对象
        /// </summary>
        [Display(Name = "选课对象")]
        public string LimitClassName { get; set; }

        /// <summary>
        /// 预选学生
        /// </summary>
        [Display(Name = "预选学生")]
        public int OrgStudentCount { get; set; }

        

        /// <summary>
        /// 分组Id
        /// </summary>
        public int ElectiveGroupId { get; set; }

        /// <summary>
        /// 分组序号
        /// </summary>
        public int ElectiveGroupNo { get; set; }

        /// <summary>
        /// 选课分组
        /// </summary>
        [Display(Name = "选课分组")]
        public string ElectiveGroupName { get; set; }

        public int ElectiveGroupMinElective { get; set; }

        /// <summary>
        /// 分组最大可选人数
        /// </summary>
        public int ElectiveGroupMaxElective { get; set; }

        /// <summary>
        /// 分段Id
        /// </summary>
        public int ElectiveSectionId { get; set; }

        public int ElectiveSectionNo { get; set; }

        [Display(Name = "选课分段")]
        public string ElectiveSectionName { get; set; }

        public int ElectiveSectionMinElective { get; set; }

        /// <summary>
        /// 分段最大可选人数
        /// </summary>
        public int ElectiveSectionMaxElective { get; set; }

        public List<string> ListWeekPeriod { get; set; } = new List<string>();

        [Display(Name = "星期节次")]
        public string WeekPeriod { get; set; }

        public int WeekId { get; set; }
        public int PeriodId { get; set; }

        public DateTime InputDate { get; set; }

        public int StudentId { get; set; }

        public string StudentCode { get; set; }

        public string StudentName { get; set; }

        public string ClassName { get; set; }

        public string HeadTeacherName { get; set; }
    }
}
