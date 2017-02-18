using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Basis.Models.Class
{
    public class ImportStudent
    {
        [Display(Name = "上传文件")]
        public string UploadFile { get; set; }

        [Display(Name = "对于系统中已存在的数据做更新操作")]
        public bool IsUpdate { get; set; }

        [Display(Name = "是否自动创建班级")]
        public bool IsAddClass { get; set; }

        public bool IsRemove { get; set; }
        
        [Display(Name = "班级")]
        public int? ClassId { get; set; }

        public List<Dto.Class.ImportStudent> ImportStudentList { get; set; } = new List<Dto.Class.ImportStudent>();

        public List<System.Web.Mvc.SelectListItem> ClassList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public bool Status { get; set; }

        [Required]
        public int YearId { get; set; } = System.Web.HttpContext.Current.Request["YearId"].ConvertToInt();
    }
}