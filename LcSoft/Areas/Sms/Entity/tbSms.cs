using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Sms.Entity
{
    /// <summary>
    /// 短信
    /// </summary>
    public class tbSms : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 短信标题
        /// </summary>
        [Display(Name = "短信标题"), Required]
        public string SmsTitle { get; set; }

        /// <summary>
        /// 验证码
        /// </summary>
        [Display(Name = "验证码")]
        public string SmsPIN { get; set; }

        /// <summary>
        /// 计划发送时间
        /// </summary>
        [Display(Name = "计划发送时间"), Required]
        public DateTime PlanDate { get; set; }

        /// <summary>
        /// 提交时间
        /// </summary>
        [Display(Name = "提交时间"), Required]
        public DateTime InputDate { get; set; }

        /// <summary>
        /// 提交人员
        /// </summary>
        [Display(Name = "提交人员"), Required]
        public virtual Sys.Entity.tbSysUser tbSysUser { get; set; }
    }
}