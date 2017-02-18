using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Course.Models.CourseDomain
{
    public class Import
    {
        [Display(Name = "上传文件")]
        public string UploadFile { get; set; }

        public bool IsUpdate { get; set; }

        public bool IsRemove { get; set; }

        public List<Dto.CourseDomain.Import> ImportList { get; set; }

        public bool Status { get; set; }
    }
}