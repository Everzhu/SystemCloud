using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Dict.Entity
{
    /// <summary>
    /// 党派
    /// </summary>
    public class tbDictParty : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 党派
        /// </summary>
        [Display(Name = "党派"), Required]
        public string PartyName { get; set; }
    }
}