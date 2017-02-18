using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Moral.Models.MoralPower
{
    public class Info
    {
        
        public int MoralItemId{ get; set; }

        [Display(Name = "德育选项")]
        public string MoralItemName { get; set; }


        [Display(Name ="班级列表")]
        public List<Dto.MoralClass.Info> MoralClass { get; set; } = new List<Dto.MoralClass.Info>();
    }
}