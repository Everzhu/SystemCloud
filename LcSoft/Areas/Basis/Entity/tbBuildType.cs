using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XkSystem.Areas.Basis.Entity
{
    /// <summary>
    /// 建筑物用途
    /// </summary>
    public class tbBuildType : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 建筑物用途
        /// </summary>
        [Required]
        [Display(Name = "建筑物用途")]
        public string BuildTypeName { get; set; }
    }
}
