using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Study.Models.Study
{
    public class Edit
    {
        public Dto.Study.Edit StudyEdit { get; set; } = new Dto.Study.Edit();

        public List<System.Web.Mvc.SelectListItem> YearList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        [Display(Name = "开始时间"), Required]
        public string ApplyFrom { get; set; }

        [Display(Name = "结束时间"), Required]
        public string ApplyTo { get; set; }
    }
}