using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Exam.Entity
{
    public class tbExamSection : Code.EntityHelper.EntityBase
    {
        [Required]
        [Display(Name = "考试学段")]
        public string ExamSectionName { get; set; }

        [Display(Name = "考试学段")]
        public string ExamSectionNameEn { get; set; }

        [Display(Name = "年级")]
        public virtual Basis.Entity.tbGrade tbGrade { get; set; }
    }
}