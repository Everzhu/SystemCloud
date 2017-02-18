using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Quality.Models.QualityOption
{
    public class Edit
    {
        //public Dto.QualityOption.Edit OptionEdit { get; set; } = new Dto.QualityOption.Edit();

        public int QualityItemId { get; set; } = System.Web.HttpContext.Current.Request["QualityItemId"].ConvertToInt();
    }
}