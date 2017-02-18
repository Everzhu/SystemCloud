using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XkSystem.Areas.Exam.Dto.ExamMark
{
    public class TagEdit
    {
        public int Id { get; set; }

        /// <summary>
        /// 学号
        /// </summary>
        [Display(Name = "学号"), Required]
        public string StudentCode { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        [Display(Name = "姓名")]
        public string StudentName { get; set; }

        /// <summary>
        /// 课程
        /// </summary>
        [Display(Name = "课程"), Required]
        public int ExamCourseId { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        [Display(Name = "状态"), Required]
        public int ExamStatusId { get; set; }
    }
}
