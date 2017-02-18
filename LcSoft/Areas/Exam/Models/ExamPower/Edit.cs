using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XkSystem.Areas.Exam.Models.ExamPower
{
    public class Edit
    {
        public Dto.ExamPower.Edit ExamPowerEdit { get; set; } = new Dto.ExamPower.Edit();

        [Display(Name = "开始时间")]
        public string FromDate { get; set; }

        [Display(Name = "结束时间")]
        public string ToDate { get; set; }
    }
}