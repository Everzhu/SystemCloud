using XkSystem.Code;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Elective.Dto.ElectiveApply
{
    public class List
    {
        public int Id { get; set; }

        [Display(Name = "课程")]
        //public virtual Course.Entity.tbCourse tbCourse { get; set; }
        public string CourseName { get; set; }

        [Display(Name = "所属选课")]
        //public virtual tbElective tbElective { get; set; }
        public string ElectiveName { get; set; }

        //[Display(Name ="申请教师")]
        //public virtual Teacher.Entity.tbTeacher tbTeacher { get; set; }

        [Display(Name = "申请人")]
        //public virtual Sys.Entity.tbSysUser tbSysUser { get; set; }
        public string UserName { get; set; }

        [Display(Name ="开班模式")]
        public bool IsMultiClass { get; set; }


        [Display(Name = "教学目标")]
        public string StudyTarger { get; set; }

        [Display(Name = "教学计划")]
        public string TeachPlan { get; set; }

        [Display(Name = "学时")]
        public int Hour { get; set; }

        [Display(Name ="学分")]
        public decimal Point { get; set; }

        [Display(Name = "最大选课人数")]
        public int MaxStudent { get; set; }

        [Display(Name = "上课教室/地点")]
        public string RoomName { get; set; }

        [Display(Name = "附件")]
        public List<Dto.ElectiveApplyFile.List> ElectiveApplyFileList { get; set; }

        /// <summary>
        /// 课程课表(星期节次)
        /// </summary>
        [Display(Name = "课程课表")]
        public List<Dto.ElectiveApplySchedule.List> ElectiveApplyScheduleList { get; set; }

        public bool IsWeekPeriod { get; set; }

        [Display(Name ="星期节次")]
        public string ElectiveApplySchedule
        {
            get
            {
                if (ElectiveApplyScheduleList == null || ElectiveApplyScheduleList.Count == 0)
                {
                    return "-";
                }
                return string.Join(",", ElectiveApplyScheduleList.Select(p => p.WeekName + p.PeriodName));
            }
        }


        [Display(Name = "申请时间")]
        public DateTime InputTime { get; set; }

        [Display(Name = "审批状态")]
        public Code.EnumHelper.CheckStatus CheckStatus { get; set; }


        [Display(Name = "审批意见")]
        public string CheckOpinion { get; set; }
    }
}