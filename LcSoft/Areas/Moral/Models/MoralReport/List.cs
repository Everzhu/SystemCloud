using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Moral.Models.MoralReport
{
    public class List
    {
        public int MoralId { get; set; } = HttpContext.Current.Request["MoralId"].ConvertToInt();

        public int ClassId { get; set; } = HttpContext.Current.Request["ClassId"].ConvertToInt();

        public DateTime? FromDate { get; set; } = HttpContext.Current.Request["FromDate"].ConvertToDateTimeWithNull();
        public DateTime? ToDate { get; set; }= HttpContext.Current.Request["FromDate"].ConvertToDateTimeWithNull();

        public List<System.Web.Mvc.SelectListItem> MoralList { get; set; } = new List<System.Web.Mvc.SelectListItem>();
        public bool MoralIsNull { get; set; }

        public bool DataIsNull { get; set; }

        public List<Dto.MoralReport.List> DataList { get; set; } = new List<Dto.MoralReport.List>();
    }
}