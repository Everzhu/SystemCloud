using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Sms.Models.Sms
{
    public class List
    {
        public List<Dto.Sms.List> SmsList { get; set; } = new List<Dto.Sms.List>();
        public Code.PageHelper Page { get; set; } = new Code.PageHelper();
        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();
        public string DateSearchFrom { get; set; } = Convert.ToString(HttpContext.Current.Request["DateSearchFrom"]);
        public string DateSearchTo { get; set; } = Convert.ToString(HttpContext.Current.Request["DateSearchTo"]);
    }
}