using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Basis.Dto.Calendar
{
    public class List
    {
        public int Id { get; set; }
        
        [Display(Name ="学年")]
        public string tbYear { get; set; }

        [Display(Name ="校历")]
        public DateTime CalendarDate { get; set; }
        
        public int tbWeekId { get; set; }

        [Display(Name ="星期")]
        public string tbWeekName { get; set; }
        
        [Display(Name ="备注")]
        public string Remark { get; set; }

    }
}