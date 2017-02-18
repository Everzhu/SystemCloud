using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace XkSystem.Areas.Exam.Dto.ExamSection
{
    public class Edit
    {
        public int Id { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        [Display(Name = "排序")]
        public int? No { get; set; }

        /// <summary>
        /// 考试学期
        /// </summary>
        [Display(Name = "学习学期"), Required]
        public string ExamSectionName { get; set; }

        /// <summary>
        /// 考试学期
        /// </summary>
        [Display(Name = "英文")]
        public string ExamSectionNameEn { get; set; }

        /// <summary>
        /// 年级
        /// </summary>
        [Display(Name = "年级")]
        public string GradeName { get; set; }

        [Display(Name = "年级"), Required]
        public int GradeId { get; set; } 
    }
}
