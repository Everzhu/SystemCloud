using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XkSystem.Areas.Dict.Entity
{
    /// <summary>
    /// 港澳台侨
    /// </summary>
    public class tbDictOverseas : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 港澳台侨
        /// </summary>
        [Display(Name = "港澳台侨"), Required]
        public string DictOverseasName { get; set; }
    }
}
