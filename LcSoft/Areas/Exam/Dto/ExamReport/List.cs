using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace XkSystem.Areas.Exam.Dto.ExamReport
{
    public class List
    {
        public int Id { get; set; }

        /// <summary>
        /// 座位号
        /// </summary>
        [Display(Name = "座位号")]
        public int? No { get; set; }

        /// <summary>
        /// 学号
        /// </summary>
        [Display(Name = "学生学号")]
        public string StudentCode { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        [Display(Name = "学生姓名")]
        public string StudentName { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        [Display(Name = "姓名")]
        public string StudentNameEn { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        [Display(Name = "性别")]
        public string SexName { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        [Display(Name = "性别")]
        public string SexNameEn { get; set; }

        public string Birthday { get; set; }
        public string BirthdayEn { get; set; }

        public string EntranceDate { get; set; }
        public string EntranceDateEn { get; set; }
        public string GradeDate { get; set; }
        public string GradeDateEn { get; set; }
        public string IssueDate { get; set; }

        [Display(Name = "修习阶段")]
        public string ExamSectionName { get; set; }

        public string ExamSectionNameEn { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        [Display(Name = "状态")]
        public int? ExamStatusId { get; set; }

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

        public int? TotalClassRank { get; set; }

        public int? TotalGradeRank { get; set; }

        public int? SegmentClassRank { get; set; }

        public int? SegmentGradeRank { get; set; }

        /// <summary>
        /// 综合成绩
        /// </summary>
        [Display(Name = "综合成绩")]
        public decimal? SegmentMark { get; set; }

        /// <summary>
        /// 综合成绩
        /// </summary>
        [Display(Name = "总分")]
        public decimal?  TotalStudentMark { get; set; }

        /// <summary>
        /// 等级
        /// </summary>
        [Display(Name = "等级")]
        public int? ExamLevelId { get; set; }

        /// <summary>
        /// 班级
        /// </summary>
        [Display(Name = "班级")]
        public string ClassName { get; set; }

        /// <summary>
        /// 班主任
        /// </summary>
        [Display(Name = "班主任")]
        public string ClassTeacherName { get; set; }

        [Display(Name = "任课老师")]
        public string OrgTeacherName { get; set; }

        /// <summary>
        /// 等级
        /// </summary>
        [Display(Name = "等级")]
        public string ExamLevelName { get; set; }

        public string ExamLevelRemark { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        [Display(Name = "状态")]
        public string ExamStatusName { get; set; }

        [Display(Name = "科目")]
        public string SubjectName { get; set; }

        public int SubjectId { get; set; }

        public int ExamCourseId { get; set; }

        public int StudentId { get; set; }

        public int ClassId { get; set; }

        public string GradeName { get; set; }

        [Display(Name = "领域")]
        public string CourseDomainName { get; set; }
        [Display(Name = "课程")]
        public string CourseName { get; set; }
        [Display(Name = "获得学分")]
        public decimal Point { get; set; }
        [Display(Name = "课程类型")]
        public string CourseTypeName { get; set; }
        [Display(Name = "等级值")]

        public decimal? ExamLevelValue { get; set; }

        [Display(Name = "任课教师")]
        public string TeacherName { get; set; }

        [Display(Name = "学分积点")]
        public decimal? StudyPoint { get; set; }

        public int ExamId { get; set; }

        public int RequirePoint { get; set; }
        public int ElectivePoint { get; set; }

        [DisplayFormat(DataFormatString = Code.Common.FormatToInt)]
        public decimal EmPoint { get; set; }
        public decimal XmPoint { get; set; }
        [Display(Name = "总分")]
        public decimal TotalEmPoint { get; set; }
        public decimal TotalXmPoint { get; set; }
    }
}
