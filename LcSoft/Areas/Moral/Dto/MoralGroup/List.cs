using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Moral.Dto.MoralGroup
{
    public class List
    {
        public int Id { get; set; }

        [Display(Name ="序号")]
        public int? No { get; set; }
        
        [Display(Name ="组名")]
        public string MoralGroupName { get; set; }
        
        [Display(Name ="德育")]
        public string MoralName { get; set; }
    }
}