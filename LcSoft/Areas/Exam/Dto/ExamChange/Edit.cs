using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace XkSystem.Areas.Exam.Dto.ExamChange
{
    public class Edit
    {
        public int Id { get; set; }

        /// <summary>
        /// 考试课程
        /// </summary>
        [Display(Name = "考试课程")]
        public string CourseName { get; set; }

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
        /// 等级
        /// </summary>
        [Display(Name = "等级")]
        public string ExamLevelName { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        [Display(Name = "状态")]
        public string ExamStatusName { get; set; }

        public int? ExamStatusId { get; set; }

        /// <summary>
        /// 考试
        /// </summary>
        [Display(Name = "考试")]
        public int ExamId { get; set; }

        /// <summary>
        /// 考试课程
        /// </summary>
        [Display(Name = "考试课程")]
        public int ExamCourseId { get; set; }

        public int? StudentId { get; set; } = System.Web.HttpContext.Current.Request["StudentId"].ConvertToInt();

        public int? LevelGroupId { get; set; }
    }
}
