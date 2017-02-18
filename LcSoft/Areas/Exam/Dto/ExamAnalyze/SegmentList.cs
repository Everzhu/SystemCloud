using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace XkSystem.Areas.Exam.Dto.ExamAnalyze
{
    public class SegmentList
    {
        public int SubjectId { get; set; }

        public int SegmentId { get; set; }

        public bool IsTotal { get; set; }

        [Display(Name = "分数段")]
        public string SegmentName { get; set; }

    }
}
