using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Survey.Models.SurveyReport
{
    public class UnSurveyClass
    {
        public List<Dto.SurveyReport.UnSurveyClass> List { get; set; } = new List<Dto.SurveyReport.UnSurveyClass>();

        public List<System.Web.Mvc.SelectListItem> SurveyList { get; set; } = new List<System.Web.Mvc.SelectListItem>();

        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();

        public int? SurveyId { get; set; } = System.Web.HttpContext.Current.Request["SurveyId"].ConvertToInt();
    }
}