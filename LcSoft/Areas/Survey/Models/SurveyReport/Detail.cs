using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace XkSystem.Areas.Survey.Models.SurveyReport
{
    public class Detail
    {
        public List<Dto.SurveyReport.Detail> SurveyDetailOrgList { get; set; } = new List<Dto.SurveyReport.Detail>();
        public List<Dto.SurveyReport.Detail> SurveyDetailClassList { get; set; } = new List<Dto.SurveyReport.Detail>();
        public List<System.Web.Mvc.SelectListItem> SurveyList { get; set; } = new List<System.Web.Mvc.SelectListItem>();
        public int SurveyId { get; set; } = System.Web.HttpContext.Current.Request["SurveyId"].ConvertToInt();
        public string SearchText { get; set; } = System.Web.HttpContext.Current.Request["SearchText"].ConvertToString();
    }
}