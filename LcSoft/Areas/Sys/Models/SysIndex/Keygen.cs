using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Sys.Models.SysIndex
{
    public class Keygen
    {
        [Display(Name = "机器码")]
        public string MachineCode { get; set; }

        [Display(Name = "注册码"), Required]
        public string Cdkey { get; set; }
    }
}