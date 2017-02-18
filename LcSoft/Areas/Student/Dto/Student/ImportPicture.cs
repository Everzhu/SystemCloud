using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Student.Dto.Student
{
    public class ImportPicture
    {
        /// <summary>
        /// 照片名称
        /// </summary>
        [Display(Name = "照片名称")]
        public string PicName { get; set; }

        /// <summary>
        /// 提示
        /// </summary>
        [Display(Name = "提示")]
        public string Error { get; set; }
    }
}