using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace XkSystem.Areas.Exam.Dto.ExamSection
{
    public class List
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
        [Display(Name = "学习时间")]
        public string ExamSectionName { get; set; }

        /// <summary>
        /// 考试学期
        /// </summary>
        [Display(Name = "英文")]
        public string ExamSectionNameEn { get; set; }

        [Display(Name = "年级")]
        public string GradeName { get; set; }
    }
}
