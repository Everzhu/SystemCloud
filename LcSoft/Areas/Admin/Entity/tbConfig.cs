using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Admin.Entity
{
    public class tbConfig : Code.EntityHelper.EntityRoot
    {
        /// <summary>
        /// 参数名称
        /// </summary>
        [Display(Name = "参数名称"), Required]
        public string ConfigName { get; set; }

        /// <summary>
        /// 配置编码
        /// </summary>
        [Display(Name = "配置编码"), Required]
        public Code.EnumHelper.ConfigType ConfigType { get; set; }

        /// <summary>
        /// 参数内容
        /// </summary>
        [Display(Name = "参数内容"), Required]
        public string ConfigValue { get; set; }
    }
}