using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Dict.Entity
{
    /// <summary>
    /// 健康状况
    /// </summary>
    public class tbDictHealth : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 健康状况
        /// </summary>
        [Display(Name = "健康状况"), Required]
        public string HealthName { get; set; }
    }
}