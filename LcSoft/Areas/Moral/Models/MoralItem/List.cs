using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using XkSystem.Code;

namespace XkSystem.Areas.Moral.Models.MoralItem
{
    public class List
    {
        public string MoralName { get; set; }

        public int MoralId { get; set; } = HttpContext.Current.Request["MoralId"].ConvertToInt();

        public List<Dto.MoralItem.List> MoralItemList { get; set; } = new List<Dto.MoralItem.List>();

        public List<SelectListItem> MoralGroupList { get; set; } = new List<SelectListItem>();

        public Code.EnumHelper.MoralType MoralType { get; set; } = Code.EnumHelper.MoralType.Once;

        public string SearchText { get; set; } = HttpContext.Current.Request["SearchText"].ConvertToString();

        public int? MoralGroupId { get; set; } = HttpContext.Current.Request["MoralGroupId"].ConvertToInt();

        public List<SelectListItem> MoralKindList { get; internal set; } = new List<SelectListItem>();

        public int? MoralKindId { get; set; } = HttpContext.Current.Request["MoralKindId"].ConvertToIntWithNull();
    }
}