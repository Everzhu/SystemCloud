using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Exam.Models.ExamSchedule
{
    public class Import
    {
        [Display(Name = "上传文件")]
        public string UploadFile { get; set; }

        [Display(Name = "对于系统中已存在的数据做更新操作")]
        public bool IsUpdate { get; set; }

        public List<Dto.ExamSchedule.Import> ImportList { get; set; } = new List<Dto.ExamSchedule.Import>();

        [Display(Name = "考试名称")]
        public int ExamId { get; set; }
    
        public List<System.Web.Mvc.SelectListItem> ExamList { get; set; } = new List<System.Web.Mvc.SelectListItem>();


        public bool Status { get; set; }
    }
}