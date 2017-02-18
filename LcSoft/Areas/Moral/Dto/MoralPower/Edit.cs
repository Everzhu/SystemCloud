using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Moral.Dto.MoralPower
{
    public class Edit
    {

        public int Id { get; set; }

        [Display(Name = "序号")]
        public int? No { get; set; }

        [Display(Name = "评价人"),Required]
        public int TeacherId { get; set; }

        [Display(Name = "评价项目"),Required]
        public int MoralItemId { get; set; }

        [Display(Name = "日期")]
        public DateTime? MoralDate { get; set; }

    }
}