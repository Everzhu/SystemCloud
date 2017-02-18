using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Elective.Dto.ElectiveApplyFile
{
    public class List
    {

        [Display(Name ="文件名")]
        public string FileName { get; set; }

        [Display(Name ="文件标题")]
        public string FileTitle { get; set; }
    }
}