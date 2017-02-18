using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Dict.Entity
{
    /// <summary>
    /// 民族
    /// </summary>
    public class tbDictNation : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 民族
        /// </summary>
        [Display(Name = "民族"), Required]
        public string NationName { get; set; }
    }
}