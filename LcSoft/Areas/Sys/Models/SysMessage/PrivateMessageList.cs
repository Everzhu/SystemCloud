using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Sys.Models.SysMessage
{
    public class PrivateMessageList
    {
        public List<Dto.SysMessage.PrivateMessageList> PrivateMessageMyList { get; set; } = new List<Dto.SysMessage.PrivateMessageList>();

        public Code.PageHelper Page { get; set; } = new Code.PageHelper();

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();
    }
}