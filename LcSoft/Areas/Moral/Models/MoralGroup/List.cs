using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Moral.Models.MoralGroup
{
    public class List
    {
        public string MoralName { get; set; }

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();
        
        public int MoralId { get; set; } = System.Web.HttpContext.Current.Request["MoralId"].ConvertToInt();

        public List<Dto.MoralGroup.List> MoralGroupList { get; set; } = new List<Dto.MoralGroup.List>();

        public Code.PageHelper Page { get; internal set; } = new Code.PageHelper();

    }
}