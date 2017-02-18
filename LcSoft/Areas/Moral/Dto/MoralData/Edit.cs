using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Moral.Dto.MoralData
{
    public class Edit
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }

        public int? No { get; set; }

        [Display(Name = "学生"), Required]
        public virtual Student.Entity.tbStudent tbStudent { get; set; }

        [Display(Name = "评分日期"), Required]
        public DateTime MoralDate { get; set; }

        [Display(Name = "德育选项"), Required]
        public virtual Entity.tbMoralItem tbMoralItem { get; set; }

        [Display(Name = "德育选项")]
        public virtual Entity.tbMoralOption tbMoralOption { get; set; }

        [Display(Name = "内容")]
        public string DataText { get; set; }

        [Display(Name = "录入时间"), Required]
        public DateTime InputDate { get; set; }

        [Display(Name = "录入人员"), Required]
        public virtual Sys.Entity.tbSysUser tbSysUser { get; set; }
    }
}