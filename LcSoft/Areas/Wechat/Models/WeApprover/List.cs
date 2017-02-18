using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Wechat.Models.WeApprover
{
    public class List
    {
        public List<Dto.WeApprover.List> RoleList { get; set; } = new List<Dto.WeApprover.List>();

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();
    }
}