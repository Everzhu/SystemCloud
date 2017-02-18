using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XkSystem.Areas.Sys.Models.SysUser
{
    public class Import
    {
        [Display(Name = "上传文件")]
        public string UploadFile { get; set; }
    }
}
