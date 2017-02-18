using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Course.Models.Schedule
{
    public class ImportClassSchedule
    {
        public int YearId { get; set; } = System.Web.HttpContext.Current.Request["YearId"].ConvertToInt();

        [Display(Name = "上传文件")]
        public string UploadFile { get; set; }

        public bool IsUpdate { get; set; }

        public bool IsRemove { get; set; }

        public List<Dto.Schedule.ImportClassSchedule> DataList { get; set; } = new List<Dto.Schedule.ImportClassSchedule>();

        public bool Status { get; set; }
    }
}