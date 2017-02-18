using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Moral.Dto.MoralStat
{
    public class Star:StudentStat
    {
        [Display(Name ="每月之星")]
        public bool IsStar { get; set; }
    }
}