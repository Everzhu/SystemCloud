using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Sys.Entity
{
    public class tbSysMessageRole: Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 所属消息
        /// </summary>
        [Display(Name = "所属消息"), Required]
        public virtual tbSysMessage tbSysMessage { get; set; }

        /// <summary>
        /// 接收角色组
        /// </summary>
        [Display(Name = "接收人员"), Required]
        public virtual tbSysRole tbSysRole { get; set; }

        
    }
}