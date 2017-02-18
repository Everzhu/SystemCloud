using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XkSystem.Areas.Sms.Entity
{
    /// <summary>
    /// 短信模板
    /// </summary>
    public class tbSmsTemplet : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 模板类型
        /// </summary>
        [Display(Name = "模板类型"), Required]
        public Code.EnumHelper.SmsTempletType SmsTempletType { get; set; }

        /// <summary>
        /// 短信模板
        /// </summary>
        [Display(Name = "短信模板"), Required]
        public string Templet { get; set; }
    }
}
