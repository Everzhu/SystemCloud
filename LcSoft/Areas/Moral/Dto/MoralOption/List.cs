using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Moral.Dto.MoralOption
{
    public class List
    {

        public int Id { get; set; }

        [Display(Name ="序号")]
        public int? No { get; set; }
        
        [Display(Name = "选项名称")]
        public string MoralOptionName { get; set; }

        /// <summary>
        /// 分值
        /// </summary>
        [Display(Name = "分值")]
        public decimal MoralOptionValue { get; set; }

        /// <summary>
        /// 所属项目Id
        /// </summary>
        public int MoralItemId { get; set; }

        [Display(Name ="所属项目")]
        public string MoralItemName { get; set; }
    }
}