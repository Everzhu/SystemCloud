using System;
using System.Collections.Generic;
using System.Web;

namespace XkSystem.Areas.Quality.Models.QualityPortrait
{
    public class Edit
    {
        public Dto.QualityPortrait.Edit QualityPortraitEdit { get; set; } = new Dto.QualityPortrait.Edit();

        public List<System.Web.Mvc.SelectListItem> YearList { get; set; } = new List<System.Web.Mvc.SelectListItem>();
    }
}