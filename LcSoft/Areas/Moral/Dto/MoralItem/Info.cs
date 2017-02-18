using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Moral.Dto.MoralItem
{
    public class Info
    {
        public int Id { get; set; }

        [Display(Name = "序号")]
        public int? No { get; set; }

        [Display(Name = "项目名称")]
        public string MoralItemName { get; set; }

        [Display(Name ="评价对象")]
        public Code.EnumHelper.MoralItemKind MoralItemKind { get; set; }

        [Display(Name = "所属德育")]
        public int MoralGroupId { get; set; }

        [Display(Name ="基础分")]
        public decimal DefaultValue { get; set; }

        [Display(Name ="最高分")]
        public decimal MaxScore { get; set; }

        [Display(Name ="最低分")]
        public decimal MinScore { get; set; }

        [Display(Name ="初始分")]
        public decimal InitScore { get; set; }
    }
}