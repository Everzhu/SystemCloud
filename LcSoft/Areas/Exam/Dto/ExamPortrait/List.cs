using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Exam.Dto.ExamPortrait
{
    public class List
    {
        public int Id { get; set; }
        /// <summary>
        /// 班级
        /// </summary>
        [Display(Name = "班级")]
        public string ClassName { get; set; }

        [Display(Name = "科目")]
        public string SubjectName { get; set; }

        [Display(Name = "考试")]
        public string ExamName { get; set; }

        [Display(Name = "学号")]
        public string StudentCode { get; set; }

        [Display(Name = "姓名")]
        public string StudentName { get; set; }

        [Display(Name = "成绩")]
        public string TotalMark { get; set; }

        [Display(Name = "班级成绩排名")]
        public string TotalClassRank { get; set; }

        [Display(Name = "年级成绩排名")]
        public string TotalGradeRank { get; set; }

        [Display(Name = "综合成绩")]
        public string SegmentMark { get; set; }

        [Display(Name = "班级综合成绩排名")]
        public string SegmentClassRank { get; set; }

        [Display(Name = "年级综合成绩排名")]
        public string SegmentGradeRank { get; set; }

        public string ClassId { get; set; }

        public string StudentId { get; set; }

        public string SubjectId { get; set; }

        public string ExamId { get; set; }

        //平均分
        public decimal AvgMark { get; set; }

        //年级排名
        public decimal GradeRank { get; set; }

        //班级排名
        public decimal ClassRank { get; set; }

        //学生人数
        public string StudentCount { get; set; }

        public decimal TotalHistory { get; set; }

        //考试成绩满分
        public decimal FullTotalMark { get; set; }

        //综合成绩满分
        public decimal FullSegmentMark { get; set; }

        //优秀人数
        public decimal ExcellentCount { get; set; }

        //优秀比率
        public string ExcellentRate { get; set; }

        //及格人数
        public decimal PassingCount { get; set; }

        //及格比率
        public string PassingRate { get; set; }

        [Display(Name = "是否优秀")]
        public string IsGood { get; set; }

        [Display(Name = "是否及格")]
        public string IsPass { get; set; }

        public decimal Status { get; set; }

        public int StudentNum { get; set; }

        /// <summary>
        /// 比率
        /// </summary>
        [Display(Name = "比率")]
        public string Rate { get; set; }

        [Display(Name = "得分")]
        public string Mark { get; set; }

        [Display(Name = "共计")]
        public int TotalCount { get; set; }
    }
}