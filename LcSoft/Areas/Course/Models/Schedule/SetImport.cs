using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace XkSystem.Areas.Course.Models.Schedule
{
    public class SetImport
    {
        /// <summary>
        /// 学年ID
        /// </summary>
        public int YearId { get; set; } = System.Web.HttpContext.Current.Request["YearId"].ConvertToInt();

        [Display(Name = "上传文件")]
        public string UploadFile { get; set; }

        public bool IsUpdate { get; set; }

        public bool IsRemove { get; set; }

        public List<Dto.Schedule.SetImport> SetImportList { get; set; } = new List<Dto.Schedule.SetImport>();

        /// <summary>
        /// 导入提示
        /// </summary>
        public List<string> ErrorMessageList { get; set; } = new List<string>();

        public bool Status { get; set; }
    }
}