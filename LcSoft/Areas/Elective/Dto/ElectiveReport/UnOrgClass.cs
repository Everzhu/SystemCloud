using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Elective.Dto.ElectiveReport
{
    public class UnOrgClass
    {
        [Display(Name ="行政班")]
        public string ClassName { get; set; }

        [Display(Name ="最大人数")]
        public int MaxLimit { get; set; }

        [Display(Name ="剩余人数")]
        public int RemainCount { get; set; }

    }
}