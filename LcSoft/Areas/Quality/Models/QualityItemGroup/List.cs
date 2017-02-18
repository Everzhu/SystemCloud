using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Quality.Models.QualityItemGroup
{
    public class List
    {
        public List<Dto.QualityItemGroup.List> QualityItemGroupList { get; set; } = new List<Dto.QualityItemGroup.List>();

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();

        public int QualityId { get; set; } = System.Web.HttpContext.Current.Request["QualityId"].ConvertToInt();
    }
}