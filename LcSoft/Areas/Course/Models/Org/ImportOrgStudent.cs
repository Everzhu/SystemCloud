using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Course.Models.Org
{
    public class ImportOrgStudent
    {
        [Display(Name = "上传文件")]
        public string UploadFile { get; set; }

        /// <summary>
        /// 对于系统中已存在的数据做更新操作
        /// </summary>
        [Display(Name = "对于系统中已存在的数据做更新操作")]
        public bool IsUpdate { get; set; }

        public List<Dto.Org.ImportOrgStudent> ImportOrgStudentList { get; set; } = new List<Dto.Org.ImportOrgStudent>();

        public bool Status { get; set; }
    }
}