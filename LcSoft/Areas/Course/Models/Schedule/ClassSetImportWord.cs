using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace XkSystem.Areas.Course.Models.Schedule
{
    public class ClassSetImportWord
    {
        public int YearId { get; set; } = System.Web.HttpContext.Current.Request["YearId"].ConvertToInt();

        [Display(Name = "上传文件")]
        public string UploadFile { get; set; }

        public bool IsUpdate { get; set; }

        public List<Dto.Schedule.ClassSetImportWord> ClassSetImportWordList { get; set; } = new List<Dto.Schedule.ClassSetImportWord>();

        public List<string> ErrorMessageList { get; set; } = new List<string>();

        public bool Status { get; set; }
    }
}