using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Dict.Entity
{
    /// <summary>
    /// 性别
    /// </summary>
    public class tbDictSex : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 性别名称
        /// </summary>
        [Display(Name = "性别"), Required]
        public string SexName { get; set; }
    }
}