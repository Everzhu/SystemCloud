using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Moral.Dto.MoralStat
{
    public class DtoBase
    {
        [Display(Name = "总分")]
        public decimal TotalScore { get; set; }
        
        [Display(Name ="总基础分")]
        public decimal TotalDefaultValue { get; set; }

        [Display(Name ="总加分")]
        public decimal TotalAddScore { get; set; }

        [Display(Name ="总扣分")]
        public decimal TotalSubScore { get; set; }

        [Display(Name = "排名")]
        public int Ranking { get; set; }

        public List<MoralItemList> MoralItemList { get; set; }
    }


    public class MoralItemList
    {
        public int Id { get; set; }

        [Display(Name = "德育选项")]
        public string MoralItemName { get; set; }

        [Display(Name = "基础分")]
        public decimal DefaultValue { get; set; }

        [Display(Name = "加分值")]
        public decimal AddScore { get; set; }

        [Display(Name = "扣分值")]
        public decimal SubScore { get; set; }

        [Display(Name = "实际得分")]
        public decimal RealScore { get; set; }
    }
}

