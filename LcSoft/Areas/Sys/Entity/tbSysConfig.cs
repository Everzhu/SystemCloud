using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace XkSystem.Areas.Sys.Entity
{
    /// <summary>
    /// 系统参数
    /// </summary>
    public class tbSysConfig : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 参数名称
        /// </summary>
        [Display(Name = "参数名称")]
        public string Title { get; set; }

        /// <summary>
        /// 配置编码
        /// </summary>
        [Display(Name = "配置编码")]
        public Code.EnumHelper.SysConfig SysConfig { get; set; }

        /// <summary>
        /// 参数内容
        /// </summary>
        [Display(Name = "参数内容")]
        public string Value { get; set; }
    }
}