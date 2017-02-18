using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Moral.Dto.MoralPower
{
    public class List
    {
        public int Id { get; set; }

        [Display(Name ="序号")]
        public int? No { get; set; }

        [Display(Name ="评价人")]
        public string TeacherName { get; set; }

        [Display(Name ="评价项目")]
        public string MoralItemName { get; set; }

        [Display(Name ="日期"),DisplayFormat(DataFormatString =Code.Common.FormatToDate)]
        public DateTime? MoralDate { get; set; }

        [Display(Name ="评价班级")]
        public string MoralPowerClassNames { get; set; }
    }
}