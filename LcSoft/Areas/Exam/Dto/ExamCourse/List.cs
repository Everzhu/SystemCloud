using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace XkSystem.Areas.Exam.Dto.ExamCourse
{
    public class List
    {
        public int Id { get; set; }

        /// <summary>
        /// 所属考试
        /// </summary>
        [Display(Name = "所属考试")]
        public string ExamName { get; set; }

        /// <summary>
        /// 考试课程
        /// </summary>
        [Display(Name = "考试课程")]
        public string CourseName { get; set; }

        /// <summary>
        /// 考试课程
        /// </summary>
        [Display(Name = "考试课程")]
        public int CourseId { get; set; }

        /// <summary>
        /// 过程分折算
        /// </summary>
        [Display(Name = "折算(%)")]
        public decimal AppraiseRate { get; set; } = 100;

        /// <summary>
        /// 过程分满分值
        /// </summary>
        [Display(Name = "满分")]
        public decimal FullAppraiseMark { get; set; } = 100;

        /// <summary>
        /// 考试成绩满分值
        /// </summary>
        [Display(Name = "折算(%)")]
        public decimal TotalRate { get; set; } = 100;

        /// <summary>
        /// 考试成绩满分值
        /// </summary>
        [Display(Name = "满分")]
        public decimal FullTotalMark { get; set; } = 100;

        /// <summary>
        /// 综合成绩满分值
        /// </summary>
        [Display(Name = "综合成绩满分")]
        public decimal FullSegmentMark { get; set; } = 100;

        /// <summary>
        /// 考试等级组
        /// </summary>
        [Display(Name = "考试等级组")]
        public string ExamLevelGroupName { get; set; }

        /// <summary>
        /// 科目
        /// </summary>
        [Display(Name = "科目")]
        public string SubjectName { get; set; }

        /// <summary>
        /// 科目
        /// </summary>
        [Display(Name = "科目")]
        public int SubjectId { get; set; }

        /// <summary>
        /// 学习时间
        /// </summary>
        [Display(Name = "学习时间")]
        public string StudyTime { get; set; }

        /// <summary>
        /// 录入状态
        /// </summary>
        [Display(Name = "录入时间")]
        public string InputDate { get; set; }

        /// <summary>
        /// 汇总设置
        /// </summary>
        [Display(Name = "汇总设置")]
        public string SetName { get; set; }

        /// <summary>
        /// 模块认定
        /// </summary>
        [Display(Name = "模块认定")]
        public bool Identified { get; set; }

        public int ExamCourseId { get; set; }

        public int IdentifyExamCourseId { get; set; }

        public int OrgId { get; set; }
        public int StudentId { get; set; }
    }
}
