using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Sms.Models.SmsConfig
{
    public class List
    {
        public List<Dto.SmsConfig.List> SmsConfigList { get; set; } = new List<Dto.SmsConfig.List>();
        public Code.PageHelper Page { get; set; } = new Code.PageHelper();
        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();
    }
}