using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Exam.Entity
{
    /// <summary>
    /// 考试等级分组
    /// </summary>
    public class tbExamSegmentGroup : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 考试分数段分组
        /// </summary>
        [Required]
        [Display(Name = "考试分数段分组")]
        public string ExamSegmentGroupName { get; set; }
    }
}