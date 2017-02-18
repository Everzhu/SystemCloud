using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Survey.Models.SurveyOption
{
    public class Edit
    {
        public Dto.SurveyOption.Edit OptionEdit { get; set; } = new Dto.SurveyOption.Edit();

        public string SurveyItemId { get; set; }
    }
}