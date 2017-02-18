using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Moral.Models.MoralItem
{
    public class Edit
    {

        public int MoralId { get; set; } = System.Web.HttpContext.Current.Request["MoralId"].ConvertToInt();

        public Dto.MoralItem.Edit MoralItemEdit { get; set; } = new Dto.MoralItem.Edit();

        public Code.EnumHelper.MoralType MoralType { get; set; } = Code.EnumHelper.MoralType.Once;

        public List<Dto.MoralOption.List> MoralOptionList { get; set; } = new List<Dto.MoralOption.List>();

        public List<SelectListItem> MoralGroupList { get; set; } = new List<SelectListItem>();

        public List<SelectListItem> MoralItemKindList { get; set; } = new List<SelectListItem>();

    }
}