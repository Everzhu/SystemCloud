using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Elective.Entity
{

    /// <summary>
    /// 选课申报
    /// </summary>
    public class tbElectiveApply : Code.EntityHelper.EntityBase
    {

        [Display(Name ="课程"),Required]
        public virtual Course.Entity.tbCourse tbCourse { get; set; }

        [Display(Name ="所属选课"),Required]
        public virtual tbElective tbElective { get; set; }

        //[Display(Name ="申请教师")]
        //public virtual Teacher.Entity.tbTeacher tbTeacher { get; set; }

        [Display(Name ="申请人"),Required]
        public virtual Sys.Entity.tbSysUser tbSysUser { get; set; }
        
        [Display(Name ="教学目标")]
        public string StudyTarger { get; set; }

        [Display(Name = "教学计划")]
        public string TeachPlan { get; set; }

        [Display(Name ="学时"),Required]
        public int Hour { get; set; }

        [Display(Name = "最大选课人数"),Required]
        public int MaxStudent { get; set; }

        [Display(Name ="上课教室/地点"),Required]
        public virtual Basis.Entity.tbRoom tbRoom { get; set; } 

        //过程评价项及分值
        [Display(Name ="过程评价项")]
        public string ProcEvaluation { get; set; }

        //[Display(Name ="附件")]
        //public virtual tbElectiveApplyFile tbElectiveApplyFile { get; set; }

        ///// <summary>
        ///// 课程课表(星期节次)
        ///// </summary>
        //[Display(Name ="课程课表")]
        //public virtual tbElectiveApplySchedule tbElectiveApplySchedule { get; set; }

        [Display(Name ="申请时间"),Required]
        public DateTime InputTime { get; set; }


        /// <summary>
        /// 当星期选课选择多个节次时，该字段用于区分申报审核通过后是开一个班级还是按节次数开多个班级
        /// </summary>
        [Display(Name = "是否开多个班级"), Required]
        public bool IsMultiClass { get; set; } = false;

        [Display(Name ="审批状态"),Required]
        public Code.EnumHelper.CheckStatus CheckStatus { get; set; }


        [Display(Name ="审批意见")]
        public string CheckOpinion { get; set; }
    }




}