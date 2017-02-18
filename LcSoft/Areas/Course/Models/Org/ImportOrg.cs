using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Course.Models.Org
{
    public class ImportOrg
    {
        [Display(Name = "上传文件")]
        public string UploadFile { get; set; }

        public List<Dto.Org.ImportOrg> ImportOrgList { get; set; } = new List<Dto.Org.ImportOrg>();

        public List<System.Web.Mvc.SelectListItem> YearList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        [Display(Name = "学年")]
        public int YearId { get; set; } = System.Web.HttpContext.Current.Request["YearId"].ConvertToInt();

        [Display(Name = "对于系统中已存在的数据做更新操作")]
        public bool IsUpdate { get; set; }

        public bool IsRemove { get; set; }

        public bool Status { get; set; }
    }
}