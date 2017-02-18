using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Admin.Entity
{
    /// <summary>
    /// 租户程序
    /// </summary>
    public class tbTenantProgram : Code.EntityHelper.EntityBase
    {
        /// <summary>
        /// 应用程序
        /// </summary>
        [Display(Name = "应用程序"), Required]
        public virtual tbProgram tbProgram { get; set; }
    }
}