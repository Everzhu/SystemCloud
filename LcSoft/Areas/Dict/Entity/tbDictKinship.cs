using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Dict.Entity
{
    /// <summary>
    /// 亲属关系
    /// </summary>
    public class tbDictKinship : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 亲属关系
        /// </summary>
        [Display(Name = "亲属关系"), Required]
        public string KinshipName { get; set; }
    }
}