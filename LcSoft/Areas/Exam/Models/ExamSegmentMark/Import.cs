using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace XkSystem.Areas.Exam.Models.ExamSegmentMark
{
    public class Import
    {
        [Display(Name = "上传文件")]
        public string UploadFile { get; set; }

        public bool Status { get; set; }
    }
}
