using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Dict.Entity
{
    /// <summary>
    /// 学位
    /// </summary>
    public class tbDictDegree : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 学位
        /// </summary>
        [Display(Name = "学位"), Key]
        public new int Id { get; set; }

        /// <summary>
        /// 学位
        /// </summary>
        [Display(Name = "学位"), Required]
        public string DegreeName { get; set; }
    }
}