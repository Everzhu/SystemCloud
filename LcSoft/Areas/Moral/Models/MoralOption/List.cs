using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Moral.Models.MoralOption
{
    public class List
    {
        public List<Dto.MoralOption.List> MoralOptionList { get; set; } = new List<Dto.MoralOption.List>();

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();

        public int MoralItemId { get; set; } = System.Web.HttpContext.Current.Request["MoralItemId"].ConvertToInt();

        public Code.PageHelper Page { get; internal set; } = new Code.PageHelper();
    }
}