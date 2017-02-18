using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace XkSystem.Areas.Exam.Dto.ExamAnalyze
{
    public class List
    {
        public int Id { get; set; }

        /// <summary>
        /// 比率
        /// </summary>
        [Display(Name = "比率")]
        public string Rate { get; set; }

        /// <summary>
        /// 班级
        /// </summary>
        [Display(Name = "班级")]
        public string ClassName { get; set; }

        [Display(Name = "科目")]
        public string SubjectName { get; set; }

        [Display(Name = "分数段")]
        public string SegmentName { get; set; }

        [Display(Name = "学号")]
        public string StudentCode { get; set; }

        [Display(Name = "姓名")]
        public string StudentName { get; set; }

        [Display(Name = "得分")]
        public string Mark { get; set; }

        [Display(Name = "等级")]
        public string LevelName { get; set; }

        [Display(Name = "共计")]
        public int TotalCount { get; set; }

        [Display(Name = "是否优秀")]
        public string IsGood { get; set; }

        [Display(Name = "是否及格")]
        public string IsPass { get; set; }

        public int SubjectId { get; set; }

        public int ClassId { get; set; }

        public int SegmentId { get; set; }

        public decimal ClassRank { get; set; }

        public string StudentCount { get; set; }

        public decimal? AvgMark { get; set; }

        public int? LevelId { get; set; }

        public int StudentId { get; set; }

        public decimal Status { get; set; }

        public int StudentNum { get; set; }

        public decimal Percent { get; set; }


        public int ExamId { get; set; }
        public int LastExamId { get; set; }
        public string ExamName { get; set; }
        public int AvgRank { get; set; }
        public int GoodRank { get; set; }
        public decimal TotalGradeRank { get; set; }
        public decimal? GradeRank { get; set; }
        public decimal? GradeLastRank { get; set; }
        public decimal? GradeAdvanceRank { get; set; }
        public decimal? StudentTotalMark { get; set; }
        public decimal? StudentLastTotalMark { get; set; }
        public int NormalRank { get; set; }
        public int PassRank { get; set; }
        public decimal? MaxMark { get; set; }
        public decimal? MinMark { get; set; }

        public decimal GoodRate { get; set; }
        public decimal NormalRate { get; set; }
        public decimal PassRate { get; set; }

        public string TeacherName { get; set; }

        public decimal? TotalMark { get; set; }
        public int TotalLevelId { get; set; }
        public int TotalLevelCount { get; set; }
        public string TotalLavelName { get; set; }
        public decimal TotalLavelMax { get; set; }
        public decimal TotalLavelMin { get; set; }

        public decimal TotalStudentCount { get; set; }
        public decimal GradeStudentCount { get; set; }
        public decimal TotalGradeStudentCount { get; set; }

        public double StandardDiff { get; set; }
        public decimal? StandardMark { get; set; }

        public decimal TotalMax { get; set; }
        public decimal TotalMin { get; set; }
    }
}
