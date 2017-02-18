using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Elective.Dto.ElectiveApply
{
    public class Edit
    {

        public int Id { get; set; }

        [Display(Name = "课程"),Required]
        public int CourseId { get; set; }

        [Display(Name ="科目"),Required]
        public int SubjectId { get; set; }


        [Display(Name = "所属选课"),Required]
        public int ElectiveId { get; set; }
        
        [Display(Name = "申请人")]
        public int TeacherId { get; set; }

        [Display(Name ="开班名称")]
        public string ElectiveOrgName { get; set; }


        [Display(Name = "教学目标")]
        public string StudyTarger { get; set; }

        [Display(Name = "教学计划")]
        public string TeachPlan { get; set; }

        [Display(Name = "学时"),Required]
        [Range(1,200,ErrorMessage ="学时必须大于0")]
        public int Hour { get; set; }

        [Display(Name = "学分"),Required]
        [Range(1, 200, ErrorMessage = "学分必须大于0")]
        public decimal Point { get; set; }

        [Display(Name = "最大选课人数"),Required]
        [Range(1, 999, ErrorMessage = "最大选课人数必须大于0")]
        public int MaxStudent { get; set; }

        [Display(Name = "上课教室/地点"),Required]
        public int RoomId { get; set; }

        [Display(Name = "是否多开")]
        public bool IsMultiClass { get; set; } = false;

        //[Display(Name = "附件")]
        public List<Dto.ElectiveApplyFile.List> tbElectiveApplyFile { get; set; }

        /// <summary>
        /// 课程课表(星期节次)
        /// </summary>
        [Display(Name = "课程课表")]
        public List<Dto.ElectiveApplySchedule.List> tbElectiveApplySchedule { get; set; }


        //[Display(Name = "星期节次")]
        //public string ElectiveApplySchedule
        //{
        //    get
        //    {
        //        if (tbElectiveApplySchedule == null || tbElectiveApplySchedule.Count == 0)
        //        {
        //            return "-";
        //        }
        //        return string.Join(",", tbElectiveApplySchedule.Select(p => p.tbWeek.WeekName + p.tbPeriod.PeriodName));
        //    }
        //}


        //[Display(Name = "申请时间")]
        //public DateTime InputTime { get; set; }

        [Display(Name = "审批状态")]
        public Code.EnumHelper.CheckStatus CheckStatus { get; set; }


        [Display(Name = "审批意见")]
        public string CheckOpinion { get; set; }


    }
}