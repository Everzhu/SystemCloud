using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace XkSystem.Areas.Moral.Models.MoralData
{
    public class List
    {
        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();

        public Code.PageHelper Page { get; set; } = new Code.PageHelper();

        public List<SelectListItem> MoralList { get; set; } = new List<SelectListItem>();

        public int? MoralId { get; set; } = System.Web.HttpContext.Current.Request["MoralId"].ConvertToInt();

        public List<Dto.MoralData.List> MoralDataList { get; set; } = new List<Dto.MoralData.List>();

        public List<SelectListItem> MoralClassList { get; set; } = new List<SelectListItem>();
    }
}