using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace XkSystem.Areas.Exam.Dto.ExamPower
{
    public class Edit
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 所属考试
        /// </summary>
        [Display(Name = "所属考试"), Required]
        public int ExamId { get; set; }

        /// <summary>
        /// 录入人员
        /// </summary>
        [Display(Name = "录入人员"), Required]
        public int TeacherId { get; set; }

        /// <summary>
        /// 录入课程
        /// </summary>
        [Display(Name = "录入课程"), Required]
        public int CourseId { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        [Display(Name = "录入开始时间")]
        public DateTime? FromDate { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        [Display(Name = "录入结束时间")]
        public DateTime? ToDate { get; set; }
    }
}
