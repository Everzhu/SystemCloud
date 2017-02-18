using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Moral.Dto.MoralStat
{
    public class List
    {   
        public decimal TotalScore { get; set; }

        [Display(Name = "总分")]
        public decimal TotalRealScore { get; set; }

        [Display(Name = "总加分")]
        public decimal TotalAddScore { get; set; }

        [Display(Name = "总扣分")]
        public decimal TotalSubScore { get; set; }

        [Display(Name = "排名")]
        public int Ranking { get; set; }

        public int StudentId { get; set; }

        [Display(Name ="学生")]
        public string StudentName { get; set; }

        public List<MoralItemUnMany> MoralItemList { get; set; } = new List<MoralItemUnMany>();

    }

    public class MoralItemUnMany
    {
        public int MoralItemId { get; set; }

        public string MoralItemName { get; set; }

        public Code.EnumHelper.MoralExpress MoralExpress { get; set; }

        public decimal MaxScore { get; set; }

        public decimal RealScore { get; set; }

        public decimal AddSocre { get; set; }

        public decimal SubScore { get; set; }
    }
}