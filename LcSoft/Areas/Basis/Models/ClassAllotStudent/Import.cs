using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Basis.Models.ClassAllotStudent
{
    public class Import
    {
        [Display(Name = "上传文件")]
        public string UploadFile { get; set; }

        [Display(Name = "对于系统中已存在的数据做更新操作")]
        public bool IsUpdate { get; set; }

        public bool IsRemove { get; set; }

        [Display(Name = "自动创建学生")]
        public bool IsAddStudent { get; set; }

        public List<Dto.ClassAllotStudent.Import> ImportStudentList { get; set; } = new List<Dto.ClassAllotStudent.Import>();

        public int YearId { get; set; } = System.Web.HttpContext.Current.Request["YearId"].ConvertToInt();

        public bool Status { get; set; }
    }
}