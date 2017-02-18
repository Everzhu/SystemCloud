using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Quality.Models.QualityItemGroup
{
    public class Edit
    {
        public Entity.tbQualityItemGroup QualityItemGroupEdit { get; set; } = new Entity.tbQualityItemGroup();

        public int QualityId { get; set; } = System.Web.HttpContext.Current.Request["QualityId"].ConvertToInt();
    }
}