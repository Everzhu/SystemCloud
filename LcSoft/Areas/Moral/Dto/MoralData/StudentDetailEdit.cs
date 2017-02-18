using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Moral.Dto.MoralData
{
    public class StudentDetailEdit
    {
        public int Id { get; set; }

        [Display(Name ="评价方式")]
        public Code.EnumHelper.MoralExpress MoralExpress { get; set; }

        [Display(Name ="分数"),Required]
        [Range(0.1,double.MaxValue,ErrorMessage ="分数必须大于0")]
        public decimal Score { get; set; }

        //[Display(Name ="备注")]
        //public string Remark { get; set; }

        [Display(Name ="评分原因")]
        public int? tbMoralDataReasonId { get; set; }

        [Display(Name = "评分原因"), Required]
        public string MoralDataReason { get; set; }

        [Display(Name ="评语"),Required]
        public string Comment { get; set; }

        [Display(Name ="图片")]
        public string MoralPhotos { get; set; }

    }
}