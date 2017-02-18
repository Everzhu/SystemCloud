using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Basis.Dto.Calendar
{
    public class Edit
    {
        public int Id { get; set; }

        [Display(Name = "学年"),Required]
        public int tbYearId { get; set; }

        [Display(Name = "校历"),Required]
        public DateTime CalendarDate { get; set; }

        [Display(Name = "星期"),Required]
        public int tbWeekId { get; set; }

        [Display(Name = "备注"),Required]
        public string Remark { get; set; }
    }
}