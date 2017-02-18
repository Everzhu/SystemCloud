using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XkSystem.Areas.Basis.Entity
{
    /// <summary>
    /// 年级类型
    /// </summary>
    public class tbGradeType : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 年级类型
        /// </summary>
        [Required]
        [Display(Name = "年级类型")]
        public string GradeTypeName { get; set; }
    }
}
