using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Quality.Models.Quality
{
    public class List
    {
        public List<Dto.Quality.List> QualityList { get; set; } = new List<Dto.Quality.List>();

        public List<System.Web.Mvc.SelectListItem> YearList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public int YearId { get; set; } = System.Web.HttpContext.Current.Request["YearId"].ConvertToInt();

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();

        public Code.PageHelper Page { get; set; } = new Code.PageHelper();
    }
}