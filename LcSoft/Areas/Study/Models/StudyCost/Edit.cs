using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Study.Models.StudyCost
{
    public class Edit
    {
        public Dto.StudyCost.Edit  StudyCostEdit { get; set; } = new Dto.StudyCost.Edit();
        public List<System.Web.Mvc.SelectListItem> TeacherList { get; set; } = new List<System.Web.Mvc.SelectListItem>();
        [Display(Name = "节次费用"), Required]
        public decimal Cost { get; set; }
    }
}