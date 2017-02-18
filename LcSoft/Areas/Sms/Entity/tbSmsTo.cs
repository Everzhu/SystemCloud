using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Sms.Entity
{
    /// <summary>
    /// 短信接收人
    /// </summary>
    public class tbSmsTo : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 所属短信
        /// </summary>
        [Display(Name = "所属短信"), Required]
        public virtual tbSms tbSms { get; set; }

        /// <summary>
        /// 接收人员
        /// </summary>
        [Display(Name = "接收人员")]
        public virtual Sys.Entity.tbSysUser tbSysUser { get; set; }

        /// <summary>
        /// 手机号码
        /// </summary>
        [Display(Name = "手机号码"), Required]
        public string Mobile { get; set; }

        /// <summary>
        /// 发出时间
        /// </summary>
        [Display(Name = "发出时间")]
        public DateTime SendDate { get; set; }

        /// <summary>
        /// 状态(-1失败，0,未发送,1,发送成功)
        /// </summary>
        [Display(Name = "状态"), Required]
        public decimal Status { get; set; }

        /// <summary>
        /// 重试次数
        /// </summary>
        [Display(Name = "重试次数"), Required]
        public int Retry { get; set; }

        /// <summary>
        /// 备注信息
        /// </summary>
        [Display(Name = "备注信息")]
        public string Remark { get; set; }
    }
}