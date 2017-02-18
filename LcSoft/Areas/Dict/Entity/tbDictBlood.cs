using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Dict.Entity
{
    /// <summary>
    /// 血型
    /// </summary>
    public class tbDictBlood : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 血型
        /// </summary>
        [Display(Name = "血型"), Required]
        public string BloodName { get; set; }
    }
}