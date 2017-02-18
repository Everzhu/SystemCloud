using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XkSystem.Areas.Dict.Entity
{
    /// <summary>
    /// 户口类型
    /// </summary>
    public class tbDictCensus : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 户口类型
        /// </summary>
        [Display(Name = "户口类型"), Required]
        public string DictCensusName { get; set; }
    }
}
