using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Moral.Dto.MoralStat
{
    public class ClassStat:DtoBase
    {
        public int ClassId { get; set; }

        [Display(Name ="班级")]
        public string ClassName { get; set; }

        /// <summary>
        /// 班级分+个人分
        /// </summary>
        [Display(Name ="总分")]
        public decimal Total { get; set; }

        [Display(Name ="总排名")]
        public int TotalRanking { get; set; }
    }
    
}