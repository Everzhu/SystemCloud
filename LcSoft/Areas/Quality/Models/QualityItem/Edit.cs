using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Quality.Models.QualityItem
{
    public class Edit
    {
        public Dto.QualityItem.Edit QualityItemEdit { get; set; } = new Dto.QualityItem.Edit();

        public int QualityItemGroupId { get; set; } = System.Web.HttpContext.Current.Request["QualityItemGroupId"].ConvertToInt();

        public List<System.Web.Mvc.SelectListItem> QualityItemGroupList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public int QualityId { get; set; } = System.Web.HttpContext.Current.Request["QualityId"].ConvertToInt();

        public List<System.Web.Mvc.SelectListItem> QualityItemTypeList { get; set; } = new List<System.Web.Mvc.SelectListItem>();
    }
}