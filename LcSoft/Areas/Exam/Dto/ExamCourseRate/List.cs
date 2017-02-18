using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace XkSystem.Areas.Exam.Dto.ExamCourseRate
{
    public class List
    {
        public int Id { get; set; }

        /// <summary>
        /// 所属考试
        /// </summary>
        [Display(Name = "考试名称")]
        public string ExamName { get; set; }

        /// <summary>
        /// 考试课程
        /// </summary>
        [Display(Name = "考试课程")]
        public string CourseName { get; set; }

        /// <summary>
        /// 过程分折算
        /// </summary>
        [Display(Name = "考试分比例(%)")]
        public decimal? Rate { get; set; } =0;

        /// <summary>
        /// 科目
        /// </summary>
        [Display(Name = "科目")]
        public string SubjectName { get; set; }

        public int ExamCourseId { get; set; }
        public int ExamCourseId1 { get; set; }
        public bool Status { get; set; }

    }
}
