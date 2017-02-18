using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Sms.Models.Sms
{
    public class Import
    {
        [Display(Name = "上传文件")]
        public string UploadFile { get; set; }

        public List<Dto.Sms.Import> SmsImportList { get; set; } = new List<Dto.Sms.Import>();
        /// <summary>
        /// 导入状态
        /// </summary>
        public bool ImportStatus { get; set; } = false;
        /// <summary>
        /// 导入类型 true:短信内容一样 false:短信内容不一样
        /// </summary>
        [Display(Name = "是否采用短信统一模式")]
        public bool ImportType { get; set; } = true;        
    }
}