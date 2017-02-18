using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Moral.Dto.MoralDataReason
{
    public class Edit
    {
        public int Id { get; set; }

        [Display(Name ="序号")]
        public int? No { get; set; }

        [Display(Name ="扣分原因"),Required]
        public string Reason { get; set; }


        [Display(Name ="德育选项"),Required]
        public int tbMoralItemId { get; set; }
    }
}