using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Moral.Dto.MoralData
{
    public class OnceList
    {
        public int Id { get; set; }

        public int? No { get; set; }

        [Display(Name ="学生")]
        public string StudentName { get; set; }
        
        [Display(Name ="评分日期")]
        [DisplayFormat(DataFormatString =Code.Common.FormatToDate)]
        public DateTime MoralDate { get; set; }
        
        public int MoralItemId { get; set; }

        [Display(Name ="项目")]
        public string MoralItemName { get; set; }


        public int MoralOptionId { get; set; }

        [Display(Name ="选项")]
        public string MoralOptionName { get; set; }
        
        [Display(Name ="内容")]
        public decimal? DataText { get; set; }
        
        [Display(Name ="录入时间")]
        public DateTime InputDate { get; set; }
                
        [Display(Name ="录入人员")]
        public string SysUserName { get; set; }

        public int StudentId { get; internal set; }

        public decimal MoralOptionScore { get; internal set; }
    }
}