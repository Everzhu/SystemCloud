using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Quality.Dto.QualityData
{
    public class Edit
    {
        public int ItemId { get; set; }

        public int OptionId { get; set; }

        public int StudentId { get; set; }

        public string QualityText { get; set; }
    }
}