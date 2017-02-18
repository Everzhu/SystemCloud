using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace XkSystem.Areas.Exam.Dto.ExamChange
{
    public class List
    {
        public int Id { get; set; }

        /// <summary>
        /// 考试课程
        /// </summary>
        [Display(Name = "考试课程")]
        public string CourseName { get; set; }

        /// <summary>
        /// 考试
        /// </summary>
        [Display(Name = "考试")]
        public string ExamName { get; set; }

        /// <summary>
        /// 座位号
        /// </summary>
        [Display(Name = "座位号")]
        public int No
        {
            get; set;
        }
        /// <summary>
        /// 学号
        /// </summary>
        [Display(Name = "学号")]
        public string StudentCode { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        [Display(Name = "姓名")]
        public string StudentName { get; set; }

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

        /// <summary>
        /// 综合成绩
        /// </summary>
        [Display(Name = "综合成绩")]
        public decimal? SegmentMark { get; set; }

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
        /// 等级
        /// </summary>
        [Display(Name = "等级")]
        public string ExamLevelName { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        [Display(Name = "状态")]
        public string ExamStatusName { get; set; }

        public int CourseId { get; set; }

        public int StudentId { get; set; }
    }
}
