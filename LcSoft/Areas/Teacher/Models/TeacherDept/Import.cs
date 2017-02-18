using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Teacher.Models.TeacherDept
{
    public class Import
    {
        [Display(Name = "上传文件")]
        public string UploadFile { get; set; }

        public List<Dto.TeacherDept.Import> ImportList { get; set; } = new List<Dto.TeacherDept.Import>();

        public bool IsUpdate { get; set; }

        [Display(Name ="是否清空现有数据")]
        public bool IsCover { get; set; }

        public bool IsRemove { get; set; }

        public bool Status { get; set; }
    }
}