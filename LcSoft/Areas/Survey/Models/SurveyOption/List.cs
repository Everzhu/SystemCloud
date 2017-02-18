using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Survey.Models.SurveyOption
{
    public class List
    {
        public List<Dto.SurveyOption.List> OptionList { get; set; } = new List<Dto.SurveyOption.List>();

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();

        public int SurveyItemId { get; set; }
    }
}