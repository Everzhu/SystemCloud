using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Exam.Entity
{
    /// <summary>
    /// 考试成绩
    /// </summary>
    public class tbExamMark : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 考试课程
        /// </summary>
        [Required]
        [Display(Name = "考试课程")]
        public virtual Exam.Entity.tbExamCourse tbExamCourse { get; set; }

        /// <summary>
        /// 学生
        /// </summary>
        [Required]
        [Display(Name = "学生")]
        public virtual Student.Entity.tbStudent tbStudent { get; set; }

        /// <summary>
        /// 考试状态
        /// </summary>
        [Display(Name = "考试状态")]
        public virtual Exam.Entity.tbExamStatus tbExamStatus { get; set; }

        /// <summary>
        /// 成绩类型（替换考试状态）
        /// </summary>
        [Display(Name = "成绩类型")]
        public Code.EnumHelper.ExamMarkTag ExamMarkTag { get; set; }

        /// <summary>
        /// 考试等级
        /// </summary>
        [Display(Name = "考试等级")]
        public virtual Exam.Entity.tbExamLevel tbExamLevel { get; set; }

        /// <summary>
        /// 任课教师
        /// </summary>
        [Display(Name = "任课教师")]
        public virtual  Teacher.Entity.tbTeacher tbTeacher { get; set; }

        /// <summary>
        /// 过程分
        /// </summary>
        [Display(Name = "过程分")]
        public decimal? AppraiseMark { get; set; }

        /// <summary>
        /// 考试成绩
        /// </summary>
        [Display(Name = "考试成绩")]
        public decimal? TotalMark { get; set; }

        /// <summary>
        /// 综合成绩
        /// </summary>
        [Display(Name = "综合成绩")]
        public decimal? SegmentMark { get; set; }

        /// <summary>
        /// 考试成绩班名
        /// </summary>
        [Display(Name = "考试成绩班名")]
        public int? TotalClassRank { get; set; }

        /// <summary>
        /// 考试成绩教学班排名
        /// </summary>
        [Display(Name = "考试成绩教学班排名")]
        public int? TotalOrgRank { get; set; }

        /// <summary>
        /// 考试成绩级名
        /// </summary>
        [Display(Name = "考试成绩级名")]
        public int? TotalGradeRank { get; set; }

        /// <summary>
        /// 综合成绩班名
        /// </summary>
        [Display(Name = "综合成绩班名")]
        public int? SegmentClassRank { get; set; }

        /// <summary>
        /// 综合成绩教学班排名
        /// </summary>
        [Display(Name = "综合成绩教学班排名")]
        public int? SegmentOrgRank { get; set; }

        /// <summary>
        /// 综合成绩级名
        /// </summary>
        [Display(Name = "综合成绩级名")]
        public int? SegmentGradeRank { get; set; }

        /// <summary>
        /// 成绩状态
        /// </summary>
        [Display(Name = "成绩状态")]
        public int IsValid { get; set; }
    }
}