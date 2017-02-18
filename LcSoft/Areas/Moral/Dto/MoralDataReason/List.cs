using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Moral.Dto.MoralDataReason
{
    public class List:Edit
    {
        
        public string tbMoralName { get; set; }

        [Display(Name ="德育选项")]
        public string tbMoralItemName { get; set; }
    }
}