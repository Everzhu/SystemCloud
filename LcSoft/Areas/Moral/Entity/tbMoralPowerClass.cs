using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XkSystem.Areas.Moral.Entity
{
    public class tbMoralPowerClass : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 所属权限
        /// </summary>
        [Required]
        [Display(Name = "所属权限")]
        public virtual tbMoralPower tbMoralPower { get; set; }

        /// <summary>
        /// 对应班级
        /// </summary>
        [Required]
        [Display(Name = "对应班级")]
        public virtual Basis.Entity.tbClass tbClass { get; set; }
    }
}
