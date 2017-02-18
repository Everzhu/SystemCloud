using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Moral.Dto.MoralGroup
{
    public class Info
    {
        public int Id { get; set; }

        [Display(Name="排序")]
        public int? No { get; set; }

        [Display(Name ="德育选项"),Required]
        public string MoralGroupName { get; set; }

        public int MoralItemCount { get; set; }
    }
}