using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using XkSystem.Code;

namespace XkSystem.Areas.Moral.Dto.MoralData
{
    public class StudentDetailList
    {
        public int Id { get; set; }

        [Display(Name ="分数")]
        public decimal Score { get; set; }
 
        [Display(Name ="操作方式")]
        public Code.EnumHelper.MoralExpress MoralExpress { get; set; }

        [Display(Name ="评分日期")]
        [DisplayFormat(DataFormatString =Code.Common.FormatToDate)]
        public DateTime Date { get; set; }


        //[Display(Name ="备注")]
        //public string Remark { get; set; }


        [Display(Name = "评分原因")]
        public string Reason { get; set; }

        [Display(Name ="评语")]
        public string Comment { get; set; }

        public bool HasPhoto { get; set; }

        [Display(Name ="审核状态")]
        public EnumHelper.CheckStatus CheckStatus { get;  set; }
    }
}