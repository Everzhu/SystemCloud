using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XkSystem.Areas.Elective.Models.Elective
{
    public class Edit
    {
        public Dto.Elective.Edit ElectiveEdit { get; set; } = new Dto.Elective.Edit();

        public List<System.Web.Mvc.SelectListItem> ElectiveTypeList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public List<System.Web.Mvc.SelectListItem> YearList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        [Display(Name = "开始时间"), Required]
        public string FromDate { get; set; }

        [Display(Name = "结束时间"), Required]
        public string ToDate { get; set; }


        [Display(Name = "申报开始时间"), Required]
        public string ApplyFromDate { get; set; }

        [Display(Name = "申报结束时间"), Required]
        public string ApplyToDate { get; set; }

    }
}
