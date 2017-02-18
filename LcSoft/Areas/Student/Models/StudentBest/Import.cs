using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Student.Models.StudentBest
{
    public class Import
    {
        [Display(Name = "上传文件")]
        public string UploadFile { get; set; }

        public List<Dto.StudentBest.Import> ImportList { get; set; } = new List<Dto.StudentBest.Import>();

        [Display(Name = "对于系统中已存在的数据做更新操作")]
        public bool IsUpdate { get; set; }

        public bool IsRemove { get; set; }

        public bool Status { get; set; }
    }
}