using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Student.Models.Student
{
    public class ImportPicture
    {
        [Display(Name = "上传文件")]
        public string UploadFile { get; set; }

        public List<Dto.Student.ImportPicture> ImportPictureList { get; set; } = new List<Dto.Student.ImportPicture>();

        [Display(Name = "对于系统中已存在的数据做更新操作")]
        public bool IsUpdate { get; set; }

        public List<System.Web.Mvc.SelectListItem> NameTypeList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        /// <summary>
        /// 照片命名方式
        /// </summary>
        [Display(Name = "照片命名方式")]
        public int NameTypeId { get; set; }

        public bool IsRemove { get; set; }

        public bool Status { get; set; }
    }
}