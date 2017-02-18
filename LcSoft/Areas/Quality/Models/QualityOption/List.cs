using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Quality.Models.QualityOption
{
    public class List
    {
        public List<Entity.tbQualityOption> OptionList { get; set; } = new List<Entity.tbQualityOption>();

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();

        public int QualityItemId { get; set; } = System.Web.HttpContext.Current.Request["QualityItemId"].ConvertToInt();
    }
}