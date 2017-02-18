using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Basis.Models.Calendar
{
    public class List
    {
        public List<Dto.Calendar.List> DataList { get; set; } = new List<Dto.Calendar.List>();

        public int? YearId { get; set; } = HttpContext.Current.Request["YearId"].ConvertToInt();

        public List<SelectListItem> YearList { get; set; } = new List<SelectListItem>();

        public DateTime? FromDate { get; set; } = HttpContext.Current.Request["FromDate"].ConvertToDateTime();

        public DateTime? ToDate { get; set; } = HttpContext.Current.Request["ToDate"].ConvertToDateTime();

        public List<SelectListItem> WeekList { get; set; } = new List<SelectListItem>();
    }
}