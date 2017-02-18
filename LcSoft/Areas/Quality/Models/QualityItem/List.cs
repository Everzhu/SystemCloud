using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Quality.Models.QualityItem
{
    public class List
    {
        public List<Dto.QualityItem.List> QualityItemList { get; set; } = new List<Dto.QualityItem.List>();

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();

        public int? QualityItemGroupId { get; set; } = System.Web.HttpContext.Current.Request["QualityItemGroupId"].ConvertToInt();

        public List<System.Web.Mvc.SelectListItem> QualityItemGroupList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public Code.PageHelper Page { get; set; } = new Code.PageHelper();

        public int QualityId { get; set; } = System.Web.HttpContext.Current.Request["QualityId"].ConvertToInt();

        public List<System.Web.Mvc.SelectListItem> QualityItemTypeList { get; set; } = new List<System.Web.Mvc.SelectListItem>();
    }
}