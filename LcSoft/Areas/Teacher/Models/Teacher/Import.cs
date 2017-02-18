using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XkSystem.Areas.Teacher.Models.Teacher
{
    public class Import
    {
        [Display(Name = "上传文件")]
        public string UploadFile { get; set; }

        public List<Dto.Teacher.Import> ImportList { get; set; }

        public bool IsUpdate { get; set; }

        public bool IsRemove { get; set; }

        public bool Status { get; set; }
    }
}
