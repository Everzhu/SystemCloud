using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Sms.Models.SmsTo
{
    public class List
    {
        public List<Dto.SmsTo.List> SmsToList { get; set; } = new List<Dto.SmsTo.List>();
        public List<System.Web.Mvc.SelectListItem> SendStatus { get; set; } = new List<System.Web.Mvc.SelectListItem>();
        public Code.PageHelper Page { get; set; } = new Code.PageHelper();
        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();
        public string DateSearchFrom { get; set; } = Convert.ToString(HttpContext.Current.Request["DateSearchFrom"]);
        public string DateSearchTo { get; set; } = Convert.ToString(HttpContext.Current.Request["DateSearchTo"]);
        public int StatusId { get; set; } = System.Web.HttpContext.Current.Request["StatusId"] == null ? -999999 : HttpContext.Current.Request["StatusId"].ConvertToInt();
    }
}