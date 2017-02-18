using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Sms.Models.SmsTo
{
    public class Detail
    {
        public List<Dto.SmsTo.Detail> SmsToDetailList { get; set; } = new List<Dto.SmsTo.Detail>();
        public int SmsId { get; set; } = System.Web.HttpContext.Current.Request["SmsId"].ConvertToInt();
    }
}