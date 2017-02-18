using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Quality.Models.QualitySelf
{
    public class Edit
    {
        public Dto.QualitySelf.Edit QualitySelfEdit { get; set; } = new Dto.QualitySelf.Edit();

        public int YearId { get; set; } = System.Web.HttpContext.Current.Request["YearId"].ConvertToInt();

        public int Type { get; set; } = System.Web.HttpContext.Current.Request["Type"].ConvertToInt();
    }
}