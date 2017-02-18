using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Basis.Models.Calendar
{
    public class Edit
    {

        public int YearId { get; set; } = HttpContext.Current.Request["YearId"].ConvertToInt();

        public DateTime CalendarDate { get; set; } = HttpContext.Current.Request["CalendarDate"].ConvertToDateTime();

        public Dto.Calendar.Edit CalendarEdit { get; set; } = new Dto.Calendar.Edit();

        public List<SelectListItem> WeekList { get; set; } = new List<SelectListItem>();

    }
}