using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XkSystem.Areas.Sys.Models.SysIndex
{
    public class SwitchUser
    {
        [Display(Name = "用户帐号"), Required]
        public string UserCode { get; set; }
    }
}
