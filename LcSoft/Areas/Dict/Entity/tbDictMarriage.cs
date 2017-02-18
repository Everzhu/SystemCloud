using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Dict.Entity
{
    /// <summary>
    /// 婚姻状况
    /// </summary>
    public class tbDictMarriage : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 婚姻状态名称
        /// </summary>
        [Display(Name = "婚姻状态"), Required]
        public string MarriageName { get; set; }
    }
}