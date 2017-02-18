using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Basis.Models.Class
{
    public class ImportClass
    {
        [Display(Name = "上传文件")]
        public string UploadFile { get; set; }

        [Display(Name = "对于系统中已存在的数据做更新操作")]
        public bool IsUpdate { get; set; }

        public bool IsRemove { get; set; }

        [Display(Name = "学年")]
        public int? YearId { get; set; }
        public List<Dto.Class.ImportClass> ImportClassList { get; set; } = new List<Dto.Class.ImportClass>();

        public List<System.Web.Mvc.SelectListItem> YearList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public bool Status { get; set; }
    }
}