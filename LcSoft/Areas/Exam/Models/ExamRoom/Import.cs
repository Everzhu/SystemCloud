using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Exam.Models.ExamRoom
{
    public class Import
    {
        [Display(Name = "上传文件")]
        public string UploadFile { get; set; }

        [Display(Name = "对于系统中已存在的数据做更新操作")]
        public bool IsUpdate { get; set; }

        public List<Dto.ExamRoom.Import> ImportList { get; set; } = new List<Dto.ExamRoom.Import>();

        public int ExamCourseId { get; set; } = System.Web.HttpContext.Current.Request["ExamCourseId"].ConvertToInt();

        public bool Status { get; set; }
    }
}