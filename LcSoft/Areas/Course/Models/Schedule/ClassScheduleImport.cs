using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;

namespace XkSystem.Areas.Course.Models.Schedule
{
    public class ClassScheduleImport
    {
        public int YearId { get; set; } = System.Web.HttpContext.Current.Request["YearId"].ConvertToInt();

        [Display(Name = "上传文件")]
        public string UploadFile { get; set; }

        public bool IsUpdate { get; set; }

        public bool IsRemove { get; set; }

        public List<string> ErrorMessageList { get; set; } = new List<string>();

        public bool Status { get; set; }
    }
}